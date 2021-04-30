using Infrastructure.Common;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.CacheManager
{
    public class CacheService
    {

        private static ConnectionMultiplexer connection = ConnectionMultiplexer.Connect(AppSetting.GetConfig("Cache:ConnectionString"));
        public static void OnConnect(Action<ISubscriber> func)
        {
            ISubscriber sub = connection.GetSubscriber();

            func(sub);
        }
    }
}
