using AutoMapper;
using IdentityServerHost.Quickstart.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using xs0910.IdentityServer4.Models;
using xs0910.IdentityServer4.ViewModels;

namespace xs0910.IdentityServer4.AutoMapper
{
    public class CustomProfile : Profile
    {
        /// <summary>
        /// 配置构造函数，创建映射关系
        /// </summary>
        public CustomProfile()
        {
            CreateMap<UserInfoDto, ApplicationUser>();
            CreateMap<RoleInfoDto, ApplicationRole>();

            CreateMap<RegisterViewModel, ApplicationUser>();

            CreateMap<ApplicationUser, EditViewModel>()
                .ForMember(dest => dest.Claims, opt => opt.Ignore());

            CreateMap<RegisterRoleViewModel, ApplicationRole>();
        }
    }
}
