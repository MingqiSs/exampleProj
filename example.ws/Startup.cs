using example.ws.Configurations;
using example.ws.Extensions;
using example.ws.Handler;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace example.ws
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
            services.AddControllers();

            services.AddSwaggerSetup();



            #region 注册 日志
            services.AddLogging(t => t.AddNLog());
            #endregion
            services.AddQuoteWebSocket(Configuration);
           
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            //KeepAliveInterval - 向客户端发送“ping”帧的频率，以确保代理保持连接处于打开状态。 默认值为 2 分钟。
            //AllowedOrigins - 用于 WebSocket 请求的允许的 Origin 标头值列表。 默认情况下，允许使用所有源。 有关详细信息，请参阅以下“WebSocket 源限制”。
            var webSocketOptions = new WebSocketOptions()
            {
                KeepAliveInterval = TimeSpan.FromSeconds(120),
                ReceiveBufferSize = 1024 * 1024 * 3
            };
            app.UseStaticFiles();
            app.UseWebSockets(webSocketOptions);

             app.UseQuoteWebSocket();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
