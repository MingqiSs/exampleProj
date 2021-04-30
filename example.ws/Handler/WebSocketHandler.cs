using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace example.ws.Handler
{
    public class WebSocketHandler
    {
        private const int bufferSize = 1024 * 1024 * 3;// 1024 * 4;
        public WebSocketConnectionManager WebSocketConnectionManager { get; set; }
        ILogger<WebSocketHandler> _logger;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="webSocketConnectionManager"></param>
        /// <param name="protobufSerializer"></param>
        public WebSocketHandler(WebSocketConnectionManager webSocketConnectionManager,
            ILogger<WebSocketHandler> logger)
        {
            WebSocketConnectionManager = webSocketConnectionManager;
            _logger = logger;
        }

        /// <summary>
        /// 获取id
        /// </summary>
        /// <returns></returns>
        public string GetId(WebSocket socket)
        {
            return WebSocketConnectionManager.GetId(socket);
        }
        /// <summary>
        /// 连接数
        /// </summary>
        /// <returns></returns>
        public int GetCount()
        {
            return WebSocketConnectionManager.GetCount();
        }
        /// <summary>
        /// 获取websocket
        /// </summary>
        /// <param name="socket"></param>
        /// <returns></returns>
        public CustomWebSocket GetCustomWebSocket(WebSocket socket)
        {
            return WebSocketConnectionManager.GetCustomWebSocket(socket);
        }
        /// <summary>
        /// 创建连接
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="uuId"></param>
        /// <param name="userId"></param>
        /// <param name="port"></param>
        /// <param name="ip"></param>
        /// <param name="stockCodes"></param>
        /// <param name="version"></param>
        public void OnConnected(WebSocket socket, string uuId, string userId, string ip = "")
        {
            WebSocketConnectionManager.AddSocket(socket, uuId, userId, ip);
        }
        /// <summary>
        /// 更新websocket信息
        /// </summary>
        /// <param name="customWebSocket"></param>
        public void UpdateCustomWebSocket(CustomWebSocket customWebSocket)
        {
            WebSocketConnectionManager.UpdateCustomWebSocket(customWebSocket);
        }
        /// <summary>
        /// 接收消息
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="webSocket"></param>
        /// <param name="handleMessage"></param>
        /// <returns></returns>
        public async Task ReceiveEntity<TEntity>(WebSocket webSocket, Action<WebSocketReceiveResult, TEntity> handleMessage) 
        {
            var buffer = new ArraySegment<byte>(new byte[bufferSize]);
            var result = await webSocket.ReceiveAsync(buffer, CancellationToken.None).ConfigureAwait(false);
            while (!result.EndOfMessage)
            {
                result = await webSocket.ReceiveAsync(buffer, default(CancellationToken)).ConfigureAwait(false);
            }
            var len = result.Count;
            var msgBuffer = buffer.Skip(0).Take(len).ToArray();
            var clineMsg = Encoding.UTF8.GetString(msgBuffer);
            var model = JsonConvert.DeserializeObject<TEntity>(clineMsg);
            handleMessage(result, model);
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="webSocket"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async ValueTask SendMessageAsync<TEntity>(WebSocket webSocket, TEntity entity)
        {
            try
            {
                lock (webSocket)
                {
                    if (webSocket.State == WebSocketState.Open)//WebSocketCanSend(webSocket)
                    {
                        //var data = _protobufSerializer.Serialize(entity); //await _protobufSerializer.SerializeAsync(entity); 
                        var data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(entity));
                        webSocket.SendAsync(new ArraySegment<byte>(data, 0, data.Length),WebSocketMessageType.Text,true, CancellationToken.None);
                    }
                }
            }
            catch (WebSocketException ex) when (ex.WebSocketErrorCode == WebSocketError.ConnectionClosedPrematurely)
            {
                _logger.LogError(ex, $"DSB(-3)发送消息,时捕获客户端异常:{ex.StackTrace}");
                await OnDisconnected(webSocket);
                //webSocket.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"DSB(-4)发送消息出现异常:{ex.StackTrace}");
            }
            //finally
            //{
            //    _sendLock.Release();
            //}
        }

        /// <summary>
        /// 发送消息给所有用户
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public Task SendMessageToAllAsync<TEntity>(TEntity entity)
        {
            var connections = WebSocketConnectionManager.GetAll();
            var tasks = new List<Task>(connections.Count);
            foreach (var pair in connections)
            {
                if (pair.Value.WebSocket.State == WebSocketState.Open)
                    tasks.Add(SendMessageAsync(pair.Value.WebSocket, entity).AsTask());
            }
            return Task.WhenAll(tasks);
        }
        /// <summary>
        /// 关闭用户连接
        /// </summary>
        /// <param name="socket"></param>
        /// <returns></returns>
        public async Task OnDisconnected(WebSocket socket)
        {
            await WebSocketConnectionManager.RemoveSocketAsync(WebSocketConnectionManager.GetId(socket));
        }
    }
}
