using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
namespace xs0910.IdentityServer4.AutoMapper
{
    /// <summary>
    /// 静态全局，AutoMapper 配置文件
    /// </summary>
    public class AutoMapperConfig
    {
        public static MapperConfiguration RegisterMapper()
        {
            return new MapperConfiguration(config =>
            {
                config.AddProfile(new CustomProfile());
            });
        }
    }
}
