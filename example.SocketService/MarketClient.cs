using example.SocketService.Sev;
using Infrastructure;
using Infrastructure.Common;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SuperSocket.ClientEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace example.SocketService
{
    /// <summary>
    /// 连接市场行情服务
    /// </summary>
    public class MarketClient
    {
        public delegate void delegate_Strategy_OnBuffer(byte[] buffer);
        public delegate_Strategy_OnBuffer SendBufferToClient;
        ILogger<MarketClient> _logger = ServiceCreater.GetLog<MarketClient>();
        private AsyncTcpSession m_wsQClient = null;
        private IPEndPoint ipp = null;
        public void Init(string ip, int port)
        {
            ipp = new System.Net.IPEndPoint(System.Net.IPAddress.Parse(ip), port);
            WSClient_ReConnect(ipp);
        }
        private object wslock = new object();
        private void WSClient_ReConnect(EndPoint remorePoint)
        {
            lock (wslock)
            {
                if ((m_wsQClient == null || !m_wsQClient.IsConnected))
                {
                    //m_wsQClient = new AsyncTcpSession();
                    //m_wsQClient.Connected += new EventHandler(WSClient__Opened);
                    //m_wsQClient.Closed += new EventHandler(WSClient__Closed);
                    //m_wsQClient.Error += new EventHandler<SuperSocket.ClientEngine.ErrorEventArgs>(WSClient__Error);
                    //m_wsQClient.DataReceived += new EventHandler<SuperSocket.ClientEngine.DataEventArgs>(WSClient_DataReceived);
                    //m_wsQClient.ReceiveBufferSize = 1024 * 16;
                    //m_wsQClient.Connect(ipp);
                    _logger.LogInformation($"启动服务");
                    //模拟行情
                    while (true)
                    {
                        var heartbeatStr = "26 00 30 32 32 35 31 33 30 30 30 35 00 00 00 00 00 00 00 00 00 00 00 BF 40 B2 73 E9 66 16 00 00 00 00 00 00 00 00";
                        Console.WriteLine(heartbeatStr);
                        var h = HexStringUtil.HexStringToByteArray(heartbeatStr);
                        SendBufferToClient(h);
                        System.Threading.Thread.Sleep(1000);
                    }
                  
                }
            }
        }
        public void SendHeadbeat()
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    bool isStart = true;
                    while (isStart)
                    {
                        if (m_wsQClient != null && m_wsQClient.IsConnected)
                        {
                            var m = new object();
                            byte[] request = SerHelper.Serialize(m);
                            _logger.LogInformation($"[SendHeadbeat] 发送心跳");
                            m_wsQClient.Send(request);
                        }
                        else
                        {
                            isStart = false;
                            break;
                        }
                        Thread.Sleep(20 * 1000);//休眠20秒
                    }
                    
                }
            });
        }
        private void WSClient__Opened(object sender, EventArgs e)
        {
            if (m_wsQClient.IsConnected && ((AsyncTcpSession)sender).LocalEndPoint == m_wsQClient?.LocalEndPoint)
            {
                _logger.LogInformation($"[WSClient__Opened] 打开服务");
                r = false;             
            }
        }
        bool r = false;
        /// <summary>
        /// 接收返回
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WSClient_DataReceived(object sender, DataEventArgs e)
        {
            byte[] receiveBuffer = new byte[e.Length];
            Buffer.BlockCopy(e.Data, 0, receiveBuffer, 0, e.Length);

            if (receiveBuffer.Length > 0)
            {
                SendBufferToClient(receiveBuffer);
            }
            else
            {
                _logger.LogInformation($"receiveBuffer Length is null");
            }
        }
        /// <summary>
        /// 手动断开连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WSClient__Closed(object sender, EventArgs e)
        {
            try
            {
                if (!((AsyncTcpSession)sender).IsConnected)
                {
                    _logger.LogInformation($"[WSClient__Closed] => WSClient_ReConnect");
                    WSClient_ReConnect(ipp);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[wsClient_Closed] : {ipp.Address} {ipp.Port}");
            }
        }
        /// <summary>
        /// 连接错误
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WSClient__Error(object sender, ErrorEventArgs e)
        {
            _logger.LogError($"[wsClient_Error]:{ipp.Address} {ipp.Port} :{e.Exception.Message},errmsg:{e.Exception.Message}");

            var heartbeatStr = "26 00 30 32 32 35 31 33 30 30 30 35 00 00 00 00 00 00 00 00 00 00 00 BF 40 B2 73 E9 66 16 00 00 00 00 00 00 00 00";

            var h = HexStringUtil.HexStringToByteArray(heartbeatStr);

            SendBufferToClient(h);//给客户端发送心跳消息

            Thread.Sleep(15 * 1000);//休眠10秒

            WSClient_ReConnect(ipp);//准备重连
        }
    }
}

