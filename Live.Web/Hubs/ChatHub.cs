using System;
using System.Linq;
using System.Threading.Tasks;
using Live.Web.Models;
using Live.Web.Store;
using Microsoft.AspNetCore.SignalR;

namespace Live.Web.Hubs
{
    public class ChatHub : Hub
    {
        private readonly UserContext _userContext;
        private readonly CommentStore _commentStore;
        private readonly OnlineStore _onlineStore;
        public ChatHub(UserContext userContext, CommentStore commentStore, OnlineStore onlineStore)
        {
            _userContext = userContext;
            _commentStore = commentStore;
            _onlineStore = onlineStore;
        }

        /// <summary>
        /// 连接
        /// </summary>
        /// <returns></returns>
        public override async Task OnConnectedAsync()
        {
            var group = GetGroupName();
            await Groups.AddToGroupAsync(Context.ConnectionId, group);
            await Clients.Group(group).SendAsync("EnterLive", _userContext.Name, "进入了直播间");

            var total = UpdateOnline(group);
            await Clients.Group(group).SendAsync("OnlineTotal", total);

            await base.OnConnectedAsync();
        }

        /// <summary>
        /// 断开
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var group = GetGroupName();
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, group);

            var total = UpdateOnline(group,false);
            await Clients.Group(group).SendAsync("OnlineTotal", total);

            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        public async Task SendMessage(string message, string group)
        {
            await Clients.Group(group).SendAsync("ReceiveMessage", _userContext.Name, message);
            _commentStore.Comments.Add(new Comment(group, _userContext.Name, message));
        }

        #region Utils

        private string GetGroupName()
        {
            var query = Context.GetHttpContext().Request.Query;
            var hasKey = query.ContainsKey("group");
            return hasKey ? query["group"].ToString() : "";
        }

        private readonly object _locker = new object();
        private int UpdateOnline(string group, bool isAdd = true)
        {
            lock (_locker)
            {
                var value = isAdd ? 1 : -1;
                var groupInfo = _onlineStore.OnlineInfo.FirstOrDefault(p => p.Group == group);
                if (groupInfo != null)
                {
                    groupInfo.Total += value;
                    return groupInfo.Total;
                }

                _onlineStore.OnlineInfo.Add(new OnlineInfo
                {
                    Group = group,
                    Total = 1
                });
                return 1;
            }
        }

        #endregion
    }
}
