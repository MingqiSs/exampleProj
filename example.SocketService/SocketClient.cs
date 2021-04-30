using example.SocketService.Sev;
using Infrastructure;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace example.SocketService
{
    /// <summary>
    /// 客户端
    /// </summary>
    public class SocketClient
    {
        public delegate void delegate_Strategy_OnClosed(string key);
        public delegate_Strategy_OnClosed RemoveSocketClient;
        ILogger<SocketClient> _logger = ServiceCreater.GetLog<SocketClient>();
        System.Threading.Thread td = null;
        public Socket clientSocket = null;
        public SocketClient(Socket _client)
        {
            clientSocket = _client;
            td = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(Receive));
            td.IsBackground = true;
            td.Start(clientSocket);
        }
        bool isstop = false;
        /// <summary>
        /// 监听客户端消息
        /// </summary>
        void Receive(object state)
        {
            Socket tmpsocket = state as Socket;
            //循环等待客户端消息
            try
            {
                while (!isstop)
                {
                    if (tmpsocket.Connected)
                    {
                        //定义一个32k大小的缓存区来接收消息流
                        byte[] receiveBuffer = new byte[1024 * 32];

                        //调用客户端链接套接字的Receive方法接收客户端发送过来的消息
                        int num = tmpsocket.Receive(receiveBuffer);

                        // 统一在此方法中判断和处理客户端发送过来的消息
                        byte[] buf = new byte[num];
                        Buffer.BlockCopy(receiveBuffer, 0, buf, 0, num);
                        ProcessMsgFromClient(tmpsocket.RemoteEndPoint, buf);
                    }
                    else
                    {
                        isstop = false;
                        //清理异常连接
                        IPEndPoint adr = tmpsocket.RemoteEndPoint as IPEndPoint;
                        string key = $"{adr.Address}:{adr.Port}";
                        _logger.LogInformation($"Client Connected {key}:{tmpsocket.Connected}");

                        RemoveSocketClient(key);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                //清理异常连接
                IPEndPoint adr = tmpsocket.RemoteEndPoint as IPEndPoint;
                string key = $"{adr.Address}:{adr.Port}";
                _logger.LogInformation($"Client Connected {key}:{ex.Message}");

                RemoveSocketClient(key);
            }
        }
        /// <summary>
        /// 统一在此方法中判断和处理客户端发送过来的消息
        /// 响应客户端心跳
        /// </summary>
        /// <param name="receiveBuffer">客户端发送过来的消息流</param>
        private void ProcessMsgFromClient(EndPoint ep, byte[] receiveBuffer)
        {
            IPEndPoint adr = ep as IPEndPoint;
            string key = $"{adr.Address}:{adr.Port}";
            //var heartbeatStr = "26 00 30 32 32 35 31 33 30 30 30 35 00 00 00 00 00 00 00 00 00 00 00 BF 40 B2 73 E9 66 16 00 00 00 00 00 00 00 00";
            //var h = HexStringUtil.HexStringToByteArray(heartbeatStr);
            //ChatMessage msg = SerHelper.Deserialize<ChatMessage>(receiveBuffer);
        }
    }
}
