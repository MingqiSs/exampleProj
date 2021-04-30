using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace example.ws.Handler
{

    public class WebSocketConnectionManager
    {
        private object _addAsync = new object();
        private object _async = new object();
        private static ConcurrentDictionary<string, CustomWebSocket> _sockets = new ConcurrentDictionary<string, CustomWebSocket>();
        /// <summary>
        /// 添加连接
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="uuId"></param>
        /// <param name="userId"></param>
        /// <param name="port"></param>
        /// <param name="ip"></param>
        public bool AddSocket(WebSocket socket, string uuId, string userId, string ip = "")
        {
            lock (_addAsync)
            {
                var model = new CustomWebSocket
                {
                    UUId = uuId,
                    UserId= userId,
                    WebSocket = socket,
                    IP = ip,

                    AddTime = DateTime.Now
                };
                _sockets.AddOrUpdate(uuId, model, (key, oldValue) => model);
                return true;
            }
        }
        /// <summary>
        /// 获取id
        /// </summary>
        /// <param name="socket"></param>
        /// <returns></returns>
        public string GetId(WebSocket socket)
        {
            return _sockets.FirstOrDefault(p => p.Value.WebSocket == socket).Key;
        }
        /// <summary>
        /// 获取总连接数
        /// </summary>
        /// <returns></returns>
        public int GetCount() => _sockets.Count();
        /// <summary>
        /// 获取客户端socket
        /// </summary>
        /// <param name="socket"></param>
        /// <returns></returns>
        public CustomWebSocket GetCustomWebSocket(WebSocket socket)
        {
            return _sockets.FirstOrDefault(p => p.Value.WebSocket == socket).Value;
        }
        /// <summary>
        /// 更新websocket
        /// </summary>
        /// <param name="customWebSocket"></param>
        public void UpdateCustomWebSocket(CustomWebSocket customWebSocket)
        {
            lock (_async)
            {
                _sockets.AddOrUpdate(customWebSocket.UUId, customWebSocket, (key, oldValue) => customWebSocket);
            }
        }
        /// <summary>
        /// 踢除用户
        /// </summary>
        /// <param name="uuId"></param>
        /// <returns></returns>
        public async Task RemoveSocketAsync(string uuId)
        {
            try
            {
                if (string.IsNullOrEmpty(uuId))
                    return;

                CustomWebSocket customWebSocket;
                _sockets.TryRemove(uuId, out customWebSocket);
                if (customWebSocket.WebSocket.State != WebSocketState.Open)
                    return;
                await customWebSocket.WebSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None).ConfigureAwait(false);
            }
            catch (Exception e)
            {

            }
        }
        /// <summary>
        /// 获取所有客户端
        /// </summary>
        /// <returns></returns>
        public ConcurrentDictionary<string, CustomWebSocket> GetAll()
        {
            return _sockets;
        }

    }
    public class CustomWebSocket
    {
        public string UUId { set; get; }
        public string UserId { get; set; }
        public WebSocket WebSocket { get; set; }
        public string Account { set; get; }
        public string IP { get; set; }

        public DateTime AddTime { get; set; }
    }


}
