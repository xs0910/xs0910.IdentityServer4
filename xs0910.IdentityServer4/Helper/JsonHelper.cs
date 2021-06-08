using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace xs0910.IdentityServer4
{
    public class JsonHelper
    {
        /// <summary>
        /// 序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string Serialize<T>(T t)
        {
            return JsonConvert.SerializeObject(t);
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T DeserializeObject<T>(string json)
        {
            return (T)JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <param name="anonymousTypeObject"></param>
        /// <returns></returns>
        public static T DeserializeAnonymousType<T>(string json, T anonymousTypeObject)
        {
            return JsonConvert.DeserializeAnonymousType<T>(json, anonymousTypeObject);
        }
    }
}
