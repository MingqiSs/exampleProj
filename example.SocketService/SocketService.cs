using Infrastructure;
using SuperSocket.ClientEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using System.Net.Sockets;
using example.SocketService.Sev;
using example.SocketService.SKSim;
using Infrastructure.Common;

namespace example.SocketService
{
    /// <summary>
    /// 服务器类
    /// 负责转发行情，只负责转发，不负责处理消息
    /// </summary>
    public class SocketService
    {
        Socket serverSocket = null;
        string host = AppSetting.GetConfig("ConnectionSocket:Server");
        int prot = Convert.ToInt32(AppSetting.GetConfig("ConnectionSocket:Port"));
        ILogger<SocketService> _logger = ServiceCreater.GetLog<SocketService>();
        public void Start(string shost, int sport)
        {

            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            //设置服务器监听终结点信息
            System.Net.IPEndPoint ipp = new System.Net.IPEndPoint(System.Net.IPAddress.Parse(shost), sport);
            serverSocket.Bind(ipp);

            //开启监听（最多10个客户端）
            serverSocket.Listen(10);
            Console.WriteLine("服务器已经启动...");
          // _logger.LogInformation("服务器已经启动...");

            //等待客户端的链接,Accept()方法是一个阻断的方法（应该放到多线程中）
            System.Threading.Thread td = new System.Threading.Thread(Accept);
            td.IsBackground = true;
            td.Start();

            System.Threading.Thread td2 = new System.Threading.Thread(USMarketStart);
            td2.IsBackground = true;
            td2.Start();
        }
        void USMarketStart()
        {
            System.Threading.Thread.Sleep(10000);//休眠5s启动
            //连接HKStock行情服务 处理心跳，向下转发
            MarketClient client = new MarketClient();
            client.SendBufferToClient += SendBufferToClient;
            client.Init(host, prot);
        }
        /// <summary>
        /// 接收行情数据，发送给各个客户端
        /// </summary>
        /// <param name="buffer"></param>
        void SendBufferToClient(byte[] buffer)
        {
            string key = "";
            try
            {
                string[] keys = SocketMan.Instance.onlineUsers.Keys.ToArray();
                foreach (string item in keys)
                {
                    key = item;
                    if (SocketMan.Instance.onlineUsers[item].clientSocket.Connected)
                    {
                        SocketMan.Instance.onlineUsers[item].clientSocket.Send(buffer);
                    }
                    else
                    {
                        SocketMan.Instance.RemoveSocket(key);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"SendBufferToClient=>{key}:{ex.Message} Total:{SocketMan.Instance.onlineUsers.Count}");
                SocketMan.Instance.RemoveSocket(key);
            }
        }
        /// <summary>
        /// 监听客户端连接
        /// </summary>
        bool isStop = false;
        void Accept()
        {
            while (!isStop)
            {
                Socket cSocket = serverSocket.Accept();
                if (cSocket.Connected)
                {
                    IPEndPoint ipp = cSocket.RemoteEndPoint as IPEndPoint;
                    string key = ipp.Address + ":" + ipp.Port;
                    try
                    {
                        //验证同一IP是否包含多个连接，如果包含，则踢掉最早的连接
                        if (SocketMan.Instance.onlineUsers.Count(s => s.Key.Contains(ipp.Address.ToString())) <=4)
                        {
                            SocketClient client = new SocketClient(cSocket);
                            client.RemoveSocketClient += RemoveSocketClient;
                            //将当前客户端的socket存到SocketManager中，管理状态
                            SocketMan.Instance.AddSocket(client);
                            _logger.LogInformation($"Accept Receive Client:{key} Total:{SocketMan.Instance.onlineUsers.Count}");
                        }
                        else
                        {
                            cSocket.Close();
                            _logger.LogInformation($"Accept Receive Client Close: {key} Total:{SocketMan.Instance.onlineUsers.Count}");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogInformation($"Accept Receive Client:{key} Total:{SocketMan.Instance.onlineUsers.Count} {ex.Message}");
                    }
                }
                else
                {
                    _logger.LogInformation($"Accept Receive:{cSocket.Connected}");
                }
            }
        }
        /// <summary>
        /// 清理异常客户端线程
        /// </summary>
        /// <param name="key"></param>
        private void RemoveSocketClient(string key)
        {
            try
            {
                SocketMan.Instance.RemoveSocket(key);
                _logger.LogInformation($"RemoveSocketClient=>{key} Total:{SocketMan.Instance.onlineUsers.Count}");
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"RemoveSocketClient=>{key}:{ex.Message}");
            }
        }
    }
}

