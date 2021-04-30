using example.ws.Handler;
using example.ws.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace example.ws.Extensions
{
    public static class WebSocketExtensions
    {
        public static IApplicationBuilder UseQuoteWebSocket(this IApplicationBuilder app)
        {
            return app.UseMiddleware<QuoteWebSocketMiddleware>();
        }
        public static IServiceCollection AddQuoteWebSocket(this IServiceCollection services, IConfiguration Configuration)
        {
            services.AddSingleton<WebSocketHandler>();
            services.AddSingleton<WebSocketConnectionManager>();
            return services;
        }
    }
}
