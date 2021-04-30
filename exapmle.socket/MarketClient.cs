
using Infrastructure.Common;
using Microsoft.Extensions.Logging;
using SuperSocket.ClientEngine;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace example.SocketClient
{
    /// <summary>
    /// 行情推送服务器
    /// </summary>
    public class MarketClient
    {
        private AsyncTcpSession m_wsQClient = null;
        private IPEndPoint ipp = null;
        private ILogger<MarketClient> _logger = ServiceCreater.GetLog<MarketClient>();
        public void Init(string ip, int port)
        {
          
            ipp = new System.Net.IPEndPoint(System.Net.IPAddress.Parse(ip), port);
            m_wsQClient = WSClient_ReConnect(ipp);
            _logger.LogInformation($"[Init]:{ipp.Address} {ipp.Port} 启动");
        }

        private AsyncTcpSession WSClient_ReConnect(EndPoint remorePoint)
        {
            //行情重连，要清空内存
            var wsClient = new AsyncTcpSession();
            wsClient.Connected += new EventHandler(WSClient__Opened);
            wsClient.Closed += new EventHandler(WSClient__Closed);
            wsClient.Error += new EventHandler<ErrorEventArgs>(WSClient__Error);
            wsClient.DataReceived += new EventHandler<DataEventArgs>(WSClient_DataReceived);
            wsClient.ReceiveBufferSize = 1024 * 64 + 2;
            wsClient.Connect(ipp);

            return wsClient;
        }
        private void WSClient__Opened(object sender, EventArgs e)
        {
            if (m_wsQClient.IsConnected && ((AsyncTcpSession)sender).LocalEndPoint == m_wsQClient?.LocalEndPoint)
            {

            }
        }
        /// <summary>
        /// 数据大小
        /// </summary>
        byte[] body = new byte[0];
        /// <summary>
        /// 固定头大小
        /// </summary>
        private readonly int haderSize = 38;
        /// <summary>
        /// 接收返回
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WSClient_DataReceived(object sender, DataEventArgs e)
        {
            try
            {
                byte[] receiveBuffer = new byte[e.Length];
                Buffer.BlockCopy(e.Data, 0, receiveBuffer, 0, e.Length);
                lock (body)
                {
                    body = ByteConvertUtil.BufferCopy(body, receiveBuffer);
                }
                do
                {
                    ushort whole_length = BitConverter.ToUInt16(body, 0);//消息长度 Message Length
                    byte[] header = new byte[haderSize];
                   
                    Buffer.BlockCopy(body, 0, header, 0, header.Length);

                    //long seqNum = BitConverter.ToInt64(header, 12);//消息的序列号

                    ushort msgCount = BitConverter.ToUInt16(header, 20);//消息数包

                    if (msgCount == 0) //refer to heartbeat
                    {
                        Console.WriteLine($"heartbeat!");
                        RemoveBodyManyLength(whole_length);
                        break;
                    }
                } while (body.Length > 38);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[Recv] WSClient_DataReceived:发生异常");
                body = new byte[0];
                return;
            }

        }
        private void RemoveBodyManyLength(int removeCount)
        {
            lock (body)
            {
                byte[] temp = new byte[body.Length - removeCount];
                Buffer.BlockCopy(body, removeCount, temp, 0, body.Length - removeCount);
                body = temp;
            }
        }
        /// <summary>
        /// 客户端关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WSClient__Closed(object sender, EventArgs e)
        {
            try
            {
                if (!((AsyncTcpSession)sender).IsConnected)
                {
                    WSClient_ReConnect(ipp);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"客户端关闭");
            }
        }
        /// <summary>
        /// 连接错误
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WSClient__Error(object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
        {
            _logger.LogError($"[wsClient_Error]:{ipp.Address} {ipp.Port} :{e.Exception.Message},errmsg:{e.Exception.Message}");

            System.Threading.Thread.Sleep(10000);

            WSClient_ReConnect(ipp);//准备重连
        }

    }
}
