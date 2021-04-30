using Infrastructure.Common;
using System;

namespace example.SocketService
{
    class Program
    {
        static void Main(string[] args)
        {
            string host = AppSetting.GetConfig("ConnectionSocket:SHost");
            int prot = Convert.ToInt32(AppSetting.GetConfig("ConnectionSocket:SPort"));
            SocketService socket = new SocketService();
            socket.Start(host, prot);

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
