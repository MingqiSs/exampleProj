using Infrastructure.Common;
using System;

namespace example.SocketClient
{
    class Program
    {
        static void Main(string[] args)
        {
            string host = AppSetting.GetConfig("ConnectionSocket:Host");
            int prot = Convert.ToInt32(AppSetting.GetConfig("ConnectionSocket:Port"));
            MarketClient socket = new MarketClient();
            socket.Init(host, prot);

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("=> ");
                Console.ForegroundColor = ConsoleColor.White;
                string cmd = Console.ReadLine();
                string[] parms = cmd.Split('.', StringSplitOptions.RemoveEmptyEntries);
                switch (parms[0])
                {
                    case "exit":
                        //todo 推出程序
                        break;
                }
            }
        }
    }
}
