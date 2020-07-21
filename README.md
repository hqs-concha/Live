### 简介

ASP.NET SignalR是为简化开发开发人员将实时web内容添加到应用程序过程而提供的类库。实时web功能指的是让服务器代码可以随时主动推送内容给客户端，而不是让服务器等待客户端的请求（才返回内容）。

SignalR 使用的三种底层传输技术分别是Web Socket, Server Sent Events 和 Long Polling.

源码地址： https://github.com/hqs-concha/Live </br>
效果地址： http://www.hqs.pub

### 新建项目

1. VS 2019中创建一个.net core 3.X的Web Mvc项目，因为.net core已经集成了SignalR，所以不需要引用任何包
2. 右键项目中的wwwroot，选择添加 -> 客户端库 -> 输入 `@microsoft/signalr@latest`， 如下：

![signalr-1](https://oss.hqs.pub/blog/signalr-1_1595318961663.png)

### 编写客户端

页面如图：

![signalr-2](https://oss.hqs.pub/blog/signalr-2_1595320845492.png)

> H5解析直播流，我是用的是Videojs，因为H5的Video标签不能直接播放`.m3u8`格式的视频

### 编写服务端

#### 集线器

``` csharp
 public class ChatHub : Hub
 {
     public override Task OnConnectedAsync()
     {

     }

     public override Task OnDisconnectedAsync(Exception exception)
     {

     }

     public async Task SendMessage(string message, string group)
     {

     }
 }
```

* 我们需要继承`Hub`类，`Hub`类中提供了三个属性：`Clients`,`Groups`,以及`Context`
* 我们可以重写他的一下两个方法
  * `OnConnectedAsync`方法，当客户端连接成功之后，我们要做的事情；
  * `OnDisconnectedAsync`方法，当客户端断开之后，我们要做的事情；
* `SendMessage`方法为自定义的方法，名字可以随便取，参数可以自定义，名称需与客户端中发送名称一致

我们可以在客户端连接成功的时候，将用户加入到对应的直播间中，并增加相应的在线人数

``` csharp
private string GetGroupName()
{
    var query = Context.GetHttpContext().Request.Query;
    var hasKey = query.ContainsKey("group");
    return hasKey ? query["group"].ToString() : "";
}

public override async Task OnConnectedAsync()
{
    var group = GetGroupName();
    await Groups.AddToGroupAsync(Context.ConnectionId, group);
    await Clients.Group(group).SendAsync("EnterLive", _userContext.Name, "进入了直播间");

    var total = UpdateOnline(group);
    await Clients.Group(group).SendAsync("OnlineTotal", total);

    await base.OnConnectedAsync();
}
```
* `group`的获取，使用客户端连接的时候，在url上定义的
* `AddToGroupAsync` 将当前连接的客户端添加至对应的组中
* `Clients.Group.SendAsync` 向对应组中所有的客户端发送消息
* `EnterLive` 为客户端监听连接成功的方法名
* `OnlineTotal` 为客户端监听实时在线人数的方法名

当客户端点击了发送消息后，我们将消息同步推送至当前房间组的所有客户端

``` csharp
public async Task SendMessage(string message, string group)
{
    await Clients.Group(group).SendAsync("ReceiveMessage", _userContext.Name, message);
    _commentStore.Comments.Add(new Comment(group, _userContext.Name, message));
}
```
* `_userContext` 为当前登录的用户信息
* `_commentStore` 为评论的数据的集合
* `SendMessage` 为客户端调用的方法，参数为客户端传入的值
* `ReceiveMessage` 为客户端监听推送新消息的方法名

当客户端关闭后，我们更新实时在线人数

``` csharp
public override async Task OnDisconnectedAsync(Exception exception)
{
    var group = GetGroupName();
    await Groups.RemoveFromGroupAsync(Context.ConnectionId, group);

    var total = UpdateOnline(group,false);
    await Clients.Group(group).SendAsync("OnlineTotal", total);

    await base.OnDisconnectedAsync(exception);
}
```

* `RemoveFromGroupAsync` 将客户端从当前组中移除
* `UpdateOnline` 更新当前在线人数

#### 配置SignalR

``` csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddControllersWithViews();
    services.AddSignalR();
}

public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
        endpoints.MapHub<ChatHub>("/home/chat-hub");
    });    
}
```

* 在`endpoints`中指定Hub相应的访问路径即可，是不是超简单的哦

### 客户端接收消息

``` js
//创建SignalR连接对象
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/home/chat-hub?group=group")
    .build();

//接收消息
connection.on("ReceiveMessage",
    function(user, message) {
        //收到推送的具体实现，user，message就是服务器推送来的信息，可以更新显示到前端页面中。
        var str = `
            <div class="live-chat-content-box">
                <div class="chat-name">${user}：</div>
                <div class="chat-msg">${message}</div>
            </div>
          `;
        appendText(str);
    });

//进入直播间
connection.on("EnterLive",
    function(user, message) {
        var str = `<div style='text-align:center;font-size:12px;'>欢迎<span style='color:red'> ${user} </span>${message}</div>`;
        appendText(str);
    });

//在线人数 
connection.on("OnlineTotal",
    function(total) {
        var online = document.querySelector("#online");
        online.innerText = total;
    });

//连接到服务器中心
connection.start().then(function() {
    console.log("chatHub connected");
}).catch(function(err) {
    console.log(err);
});
```

发送消息

``` js
function send() {
    var message = document.querySelector("#message");
    if (!message.value) return;
    connection.invoke("SendMessage", message.value, 'group').catch(function(err) {
        console.log(err);
    });
    message.value = "";
}
```

* 接收消息的名称，与服务端中`Clients.Group.SendAsync`中配置的名称要匹配
* 发送消息的名称，与服务端中`ChatHub`类中的`SendMessage`名称对应

### 发布

进入项目根目录，执行 `dotnet publish -c release`

服务器采用的是centos，使用nginx做方向代理，将文件拷贝至服务器后，需要额外配置nginx

``` nginx
location / {
    proxy_pass http://127.0.0.1:5000;
}

location /home/chat-hub {
    proxy_pass http://127.0.0.1:5000/home/chat-hub;
    proxy_set_header Upgrade $http_upgrade;
    proxy_set_header Connection upgrade;
    proxy_http_version 1.1;
}
```

* 第一个`location`配置的是代理当前网站
* 第二个`location`后面填写的`/home/chat-hub`为signalR地址，这个需要单独配置，不然跑不通， 因为signalR需要长连接, 而其他请求未必需要

### 效果图

![signalr-3](https://oss.hqs.pub/blog/signalr-3_1595322254336.png)
![signalr-4](https://oss.hqs.pub/blog/signalr-4_1595322272151.png)

源码地址： https://github.com/hqs-concha/Live </br>
效果地址： http://www.hqs.pub
