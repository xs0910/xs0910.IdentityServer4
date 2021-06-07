using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace xs0910.IdentityServer4
{
    /// <summary>
    /// 内存模式
    /// </summary>
    public class InMemoryConfig
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
                new IdentityResources.Profile()
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
            return new[]
            {
                new ApiResource("xs0910.Core.API","xs0910.Core.API")
                {
                    Scopes = { "scope" }
                }
            };
        }

        /// <summary>
        /// 客户端：哪些客户端 Client 应用可以使用这个Authorization Server
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Client> GetClients()
        {
            return new[]
            {
                new Client
                {
                    // 客户端Id
                    ClientId = "vue.js",
                    // client 用来获取token的密钥
                    ClientSecrets =new []{ new Secret("secret".Sha256()) },
                    // 这里使用的是通过用户名密码和ClientCredentials来换取token的方式。
                    // ClientCredentials允许Client只使用ClientSecrets来获取token。
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                    // 允许访问的 API 资源
                    AllowedScopes = new []{ "scope" }
                }
            };
        }

        /// <summary>
        /// Users：指定可以使用 Authorization Server 授权的Users 用户
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<TestUser> Users()
        {
            return new[]
            {
                new TestUser
                {
                    SubjectId = "1",
                    Username = "admin",
                    Password = "admin"
                },
                new TestUser
                {
                    SubjectId ="2",
                    Username = "xs0910",
                    Password = "xs0910"
                }
            };
        }
    }
}
