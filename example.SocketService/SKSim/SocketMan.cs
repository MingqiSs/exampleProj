using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace example.SocketService.SKSim
{
    public class SocketMan
    {
        private static readonly SocketMan _instance = new SocketMan();
        private SocketMan() { }

        /// <summary>
        /// 此字典用来存储当前所有在线客户端的链接套接字
        /// key:客户端ip+":"+port
        /// value:客户端的连接套接字对象
        /// </summary>
        Dictionary<string, SocketClient> diclist = new Dictionary<string, SocketClient>();

        public static SocketMan Instance
        {
            get
            {
                return _instance;
            }
        }

        /// <summary>
        /// 当客户端链接上服务器的时候，添加到diclist
        /// </summary>
        /// <param name="csocket"></param>
        public void AddSocket(SocketClient csocket)
        {
            IPEndPoint ipp = csocket.clientSocket.RemoteEndPoint as IPEndPoint;
            string key = ipp.Address + ":" + ipp.Port;
            lock (diclock)
            {
                if (!diclist.ContainsKey(key))
                {
                    diclist.Add(key, csocket);
                }
            }
        }
        private object diclock = new object();
        /// <summary>
        /// 当客户端关闭的时候,Socket移除
        /// </summary>
        /// <param name="csocket"></param>
        public void RemoveSocket(string key)
        {
            if (diclist.ContainsKey(key))
            {
                lock (diclock)
                {
                    if (diclist.ContainsKey(key))
                    {
                        diclist.Remove(key);
                    }
                }
            }
        }

        /// <summary>
        /// 获取当前在线用户集合
        /// </summary>
        public Dictionary<string, SocketClient> onlineUsers
        {
            get
            {
                return this.diclist;
            }
        }
    }
}
