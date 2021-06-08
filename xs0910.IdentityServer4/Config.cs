using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace xs0910.IdentityServer4
{
    public class Config
    {

        /// <summary>
        /// Identity Resources
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new IdentityResource[]
            {
                new IdentityResources.OpenId(),         // 必填：不然会报错
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResource("roles","角色",new List<string>{ JwtClaimTypes.Role })
            };
        }

        /// <summary>
        /// API Scopes：指定 Authorization Server允许访问的API Scopes
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ApiScope> GetApiScopes()
        {
            return new[]
            {
                new ApiScope("scope")
            };
        }

        /// <summary>
        /// API资源：Authorization Server保护了哪些 API 资源
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new ApiResource[]
            {
                new ApiResource("xs0910.Core.API","xs0910.Core.API")
                {
                    UserClaims = { JwtClaimTypes.Name, JwtClaimTypes.Role },
                    Scopes = { "scope" },
                    ApiSecrets = { new Secret("secret".Sha256()) }
                }
            };
        }

        /// <summary>
        /// 客户端：哪些客户端 Client 应用可以使用这个Authorization Server
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Client> GetClients()
        {
            return new Client[]
            {
                // 1. vue项目
                new Client
                {
                    // 客户端Id
                    ClientId = "vue.js",
                    ClientName ="Vue Client",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser =true,
                    // 跳转地址
                    RedirectUris = { "http://localhost:6688/callback" },
                    // 登出地址
                    PostLogoutRedirectUris = { "http://localhost:6688" },
                    // 允许跨域地址
                    AllowedCorsOrigins = { "http://localhost:6688" },
                    // Token时效
                    AccessTokenLifetime = 3600,
                    // 允许访问的 API 资源
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "roles",
                        "scope"
                    }
                },

                // 2.控制台客户端
                new Client
                {
                    ClientId = "Console",
                    // client 用来获取token的密钥
                    ClientSecrets={ new Secret("secret".Sha256()) },
                    AllowedGrantTypes = new List<string>()
                    {
                        GrantTypes.ResourceOwnerPassword.FirstOrDefault(),
                        GrantTypes.ClientCredentials.FirstOrDefault()
                    },
                    AllowedScopes =new List<string>
                    {
                       IdentityServerConstants.StandardScopes.OpenId,
                       IdentityServerConstants.StandardScopes.Profile,
                       "scope"
                    }
                },
            };

        }

    }
}
