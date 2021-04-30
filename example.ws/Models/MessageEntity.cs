using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace example.ws.Models
{
    public class MessageEntity
    {
        /// <summary>
        /// 消息内容
        /// </summary>
        /// <value></value>
        public string D { get; set; }

        /// <summary>
        /// 消息类型
        /// </summary>
        /// <value></value>
        public int T { get; set; }
        /// <summary>
        /// 请求ID
        /// </summary>
        public string R { get; set; }
        /// <summary>
        /// 版本
        /// </summary>
        //[ProtoMember(4)]
        //public int V { set; get; } = 0;
    }
    public enum MessageType
    {
        /// <summary>
        /// 连接
        /// </summary>
        Connection = 1,
        /// <summary>
        /// 行情
        /// </summary>
        Quote = 3,
        /// <summary>
        /// 登录
        /// </summary>
        Login = 4,
        /// <summary>
        /// 失败
        /// </summary>
        Fail=5,
        /// <summary>
        /// 修改密码
        /// </summary>
        ChangePassword = 91,
        /// <summary>
        /// 重置密码
        /// </summary>
        ResetPassword = 92,
        /// <summary>
        /// 修改密码
        /// </summary>
        ChangeUserPassword = 93,
        /// <summary>
        /// 重置密码
        /// </summary>
        ResetUserPassword = 94,
        /// <summary>
        /// 登出
        /// </summary>
        Logout = 99,
    }
}
