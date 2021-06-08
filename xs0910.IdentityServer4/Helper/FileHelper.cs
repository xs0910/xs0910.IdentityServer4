using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xs0910.IdentityServer4
{
    public class FileHelper
    {
        /// <summary>
        /// 读取文件信息
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        public static string ReadFile(string path)
        {
            string result = "";
            if (!File.Exists(path))
            {
                return result;
            }

            using StreamReader stream = new StreamReader(path, Encoding.UTF8);
            result = stream.ReadToEnd();
            return result;
        }
    }
}
