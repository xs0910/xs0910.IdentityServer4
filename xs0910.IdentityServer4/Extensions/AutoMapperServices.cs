using Microsoft.Extensions.DependencyInjection;
using System;
using xs0910.IdentityServer4.AutoMapper;
using AutoMapper;
namespace xs0910.IdentityServer4
{
    public static class AutoMapperServices
    {
        public static void AddAutoMapper(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddAutoMapper(typeof(AutoMapperConfig));
            AutoMapperConfig.RegisterMapper();
        }
    }
}
