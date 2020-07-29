using System;
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
            Console.WriteLine($"数据库连接：{Configuration["DB_CONNECTION"]}");
            Console.WriteLine($"数据库名称：{Configuration["DB_NAME"]}");
            services.AddControllersWithViews();
            services.AddSession();
            services.AddHttpContextAccessor();
            services.AddSingleton<UserStore>();
            services.AddSingleton<CommentStore>();
            services.AddSingleton<OnlineStore>();
            services.AddScoped<UserContext>();
            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseStaticFiles();
            app.UseSession();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapHub<ChatHub>("/home/chat-hub");
            });
        }
    }
}
