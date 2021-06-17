using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace xs0910.IdentityServer4.ViewModels
{
    /// <summary>
    /// 返回消息
    /// </summary>
    public class MessageResult
    {
        public MessageResult(int code = 200, bool success = true, string msg = "成功")
        {
            Code = code;
            Success = success;
            Msg = msg;
        }

        /// <summary>
        /// 状态码
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 返回消息
        /// </summary>
        public string Msg { get; set; }

        public static MessageResult Failure(string msg)
        {
            return new MessageResult(201, false, msg);
        }
    }

    /// <summary>
    /// 返回消息
    /// </summary>
    /// <typeparam name="T">泛型</typeparam>
    public class MessageResult<T> : MessageResult
    {
        public MessageResult(T data, int code = 200, bool success = true, string msg = "成功") : base(code, success, msg)
        {
            Data = data;
        }

        public T Data { get; set; }
    }
}
