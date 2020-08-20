using System;
using Consul;
using Live.Web.Helper;
using Live.Web.Hubs;
using Live.Web.Models;
using Live.Web.Store;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Live.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            LogHelper.Write("!!!oh shit!!!");
            Console.WriteLine($"���ݿ����ӣ�{Configuration["DB_CONNECTION"]}");
            Console.WriteLine($"���ݿ����ƣ�{Configuration["DB_NAME"]}");
            services.AddControllersWithViews();
            services.AddSession();
            services.AddHttpContextAccessor();
            services.AddSingleton<UserStore>();
            services.AddSingleton<CommentStore>();
            services.AddSingleton<OnlineStore>();
            services.AddScoped<UserContext>();
            services.AddSignalR();
            services.AddHealthChecks();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseStaticFiles();
            app.UseSession();

            app.UseRouting();
            app.UseHealthChecks("/health");

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapHub<ChatHub>("/home/chat-hub");
            });

            RegisterConsul();
        }

        private void RegisterConsul()
        {
            var client = new ConsulClient(config =>
            {
                config.Address = new Uri("http://x.y:8500");
                //config.Datacenter = "dc1";
            });

#if DEBUG
            var port = 5000;
            var ip = "x.y";
#else
            var port = int.Parse(Configuration["SERVICE_PORT"]);
            var ip = Configuration["SERVICE_IP"];
#endif

            client.Agent.ServiceRegister(new AgentServiceRegistration
            {
                ID = Guid.NewGuid().ToString(), //�����ţ������ظ�
                Name = "Live",                //��������  
                Port = port,
                Address = ip,
                Check = new AgentServiceCheck
                {
                    DeregisterCriticalServiceAfter = TimeSpan.FromMilliseconds(1), //����ֹͣ����ע��
                    Interval = TimeSpan.FromSeconds(5), //���񽡿������
                    Timeout = TimeSpan.FromSeconds(5), //��鳬ʱ��ʱ��
                    HTTP = $"http://{port}:{ip}/health" //���ĵ�ַ
                }
            });
        }
    }
}
