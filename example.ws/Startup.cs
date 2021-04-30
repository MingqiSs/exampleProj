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



            #region ע�� ��־
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
            //KeepAliveInterval - ��ͻ��˷��͡�ping��֡��Ƶ�ʣ���ȷ�����������Ӵ��ڴ�״̬�� Ĭ��ֵΪ 2 ���ӡ�
            //AllowedOrigins - ���� WebSocket ���������� Origin ��ͷֵ�б� Ĭ������£�����ʹ������Դ�� �й���ϸ��Ϣ����������¡�WebSocket Դ���ơ���
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
