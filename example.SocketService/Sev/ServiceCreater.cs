using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace example.SocketService.Sev
{
    public class ServiceCreater
    {
        public static ILogger<T> GetLog<T>()
        {
            return new ServiceCollection()
                    .AddLogging(t => t.AddNLog())
                    .BuildServiceProvider().GetService<ILogger<T>>();
        }
    }
}
