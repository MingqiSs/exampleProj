using example.ws.Handler;
using example.ws.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace example.ws.Middleware
{

    public partial class QuoteWebSocketMiddleware
    {
        public RequestDelegate _next;
        public WebSocketHandler _webSocketHandler;
        public const string routePostfix = "/quote";
        public const int MaxConnection = 2000;
        public ILogger<QuoteWebSocketMiddleware> _logger;
        public QuoteWebSocketMiddleware(RequestDelegate next,
            WebSocketHandler webSocketConnectionManager,
            ILogger<QuoteWebSocketMiddleware> logger)
        {
            _next = next;
            _webSocketHandler = webSocketConnectionManager;
            _logger = logger;
        }
        public async Task Invoke(HttpContext context)
        {
            if (!(context.WebSockets.IsWebSocketRequest || context.Request.Path == routePostfix))
            {
                await _next.Invoke(context);
            }
            string uuId = context.Request.Query["u"];
            if (string.IsNullOrEmpty(uuId))
            {
                await _next.Invoke(context);
                context.Response.StatusCode = 401;
                return;
            }
            string ip = string.Empty;
            var count = _webSocketHandler.GetCount();
            _logger.LogInformation($"连接uuId:{uuId},ip:{ip},count:{count}");
            if (count >= MaxConnection)
            {
                await _next.Invoke(context);
                context.Response.StatusCode = 501;
                return;
            }
            //创建webSocket连接
            var socket = await context.WebSockets.AcceptWebSocketAsync();
            _webSocketHandler.OnConnected(socket, uuId, string.Empty, ip);
            try
            {
                while (socket.State == WebSocketState.Open)//socket.State == WebSocketState.Open
                {
                    await _webSocketHandler.ReceiveEntity<MessageEntity>(socket, async (result, messageEntity) =>
                    {
                        if (result.MessageType == WebSocketMessageType.Close)
                        {
                            await _webSocketHandler.OnDisconnected(socket);
                            await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Close", default(CancellationToken));
                            return;
                        }
                        var customSocket = _webSocketHandler.GetCustomWebSocket(socket);
                        if (customSocket == null)
                        {
                            await _webSocketHandler.OnDisconnected(socket);
                            await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Close", default(CancellationToken));
                            _logger.LogInformation($"未找到相应连接:{JsonConvert.SerializeObject(messageEntity)},State={socket?.State}");
                            return;
                        }
                        var code = (MessageType)messageEntity.T;
                        var method = code.ToString();
                        if (this.GetType().GetMethods().Where(p => p.Name == method).FirstOrDefault() == null)
                        {
                            messageEntity.D = "指令错误";
                            messageEntity.T = (int)MessageType.Fail;
                            await _webSocketHandler.SendMessageAsync(socket, messageEntity);
                            return;
                        }
                        var pars = new object[3];
                        pars[0] = socket;
                        pars[1] = customSocket;
                        pars[2] = messageEntity;
                        InvokeInstruction(code.ToString(), pars);
                    });
                }
            }
            catch (WebSocketException ex) when (ex.WebSocketErrorCode == WebSocketError.ConnectionClosedPrematurely)
            {
                await _webSocketHandler.OnDisconnected(socket);
            }
            catch (OperationCanceledException)
            {
                // Ignore aborts, don't treat them like transport errors
            }
            catch (Exception ex)
            {
                var customId = _webSocketHandler.GetId(socket);
                _logger.LogError(ex, $"DSB(-2)系统接收({customId}:{socket.State})消息出现异常:{ex.StackTrace}");
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="parameters"></param>
        private void InvokeInstruction(string methodName, object[] parameters)
        {
            this.GetType().GetMethod(methodName).Invoke(this, parameters);
        }

    }
}
