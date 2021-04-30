using example.ws.Handler;
using example.ws.Models;
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
        /// <summary>
        /// 行情
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="customSocket"></param>
        /// <param name="messageEntity"></param>
        /// <returns></returns>
        public async Task Quote(WebSocket socket, CustomWebSocket customSocket, MessageEntity messageEntity)
        {
            // CacheService.OnConnect((sub) =>
            //{
            //    sub.Subscribe("tick_us", async (channel, message) =>
            //   {
            //       var a = message.ToString().Split('|');
            //       var SecurityCode = a[0];
            //       var price = Convert.ToInt32(a[1]);
            //       var tradePrice = price;
            //       var tradeSize = Convert.ToInt32(a[2]);
            //       var utcTick = Convert.ToInt64(a[3]);
            //       var flagUp = a[4];
            //       var type = a[5];//2:美股  
            //       var Ttype = a[6];//交易类型:0  
            //       var msg = string.Format("{0} | {1} |  {2}  | {3}   \r\n", Utils.ConvertDateTime(utcTick).ToString("HH:mm:ss"),
            //                   ConverPrice(tradePrice),
            //                   flagUp,
            //                   String.Format("{0:D4}", tradeSize)
            //                  );
            //       msg = $"{msg} |{SecurityCode}";
            //       messageEntity.D = msg;
            //       await _webSocketHandler.SendMessageAsync(socket, messageEntity);
            //   });
            //});
          ///模拟行情
            while (true)
            {
                Thread.Sleep(100);
                var securityCode = "AAPL";
                var msg = string.Format("{0} | {1} |  {2}  | {3}   \r\n", DateTime.Now.ToString("HH:mm:ss"),
                                123,
                                0,
                                String.Format("{0:D4}", 5555)
                               );
                msg = $"{msg} |{securityCode}";
                messageEntity.D = msg;
                await _webSocketHandler.SendMessageAsync(socket, messageEntity);
            }
            // _webSocketHandler.UpdateCustomWebSocket(customSocket);
        }


        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="customSocket"></param>
        /// <param name="messageEntity"></param>
        /// <returns></returns>
        public async Task Login(WebSocket socket, CustomWebSocket customSocket, MessageEntity messageEntity)
        {
            if (string.IsNullOrEmpty(messageEntity.D))
            {
                messageEntity.D = "请输入参数";
                await _webSocketHandler.SendMessageAsync(socket, messageEntity);
                return;
            }
            try
            {
                //var tradeLoginRQ = JsonConvert.DeserializeObject<LoginRQ>(messageEntity.D);
                messageEntity.D = "登录成功";
                await _webSocketHandler.SendMessageAsync(socket, messageEntity);
                return;
            }
            catch
            {
                
            }
        }
        public static string ConverPrice(int p)
        {
            return $"{Math.Round((decimal)p / 1000, 3).ToString("0.000")}";
        }
    }
    public class LoginRQ
    {
        /// <summary>
        /// 交易账号
        /// </summary>
        public string ac { set; get; }
        /// <summary>
        /// 交易密码
        /// </summary>
        public string p { set; get; }
        /// <summary>
        /// 保持在线时长(单位：分钟)
        /// </summary>
        public int m { set; get; } = 5;
        /// <summary>
        /// 设备号
        /// </summary>
        public string d { set; get; }
        /// <summary>
        /// 验证数据
        /// </summary>
        public string vd { set; get; }
        /// <summary>
        /// 验证类型(0:密码登录 1:手机校验)
        /// </summary>
        public int vt { set; get; } = 0;
    }
}
