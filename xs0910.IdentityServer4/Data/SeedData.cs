using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using xs0910.IdentityServer4.Models;

namespace xs0910.IdentityServer4.Data
{
    /// <summary>
    /// 种子数据
    /// </summary>
    public class SeedData
    {
        /// <summary>
        /// 执行 dotnet run /seed
        /// </summary>
        /// <param name="serviceProvider"></param>
        public static void EnsureSeedData(IServiceProvider serviceProvider)
        {
            Console.WriteLine("Seeding Database...");

            using (var scope = serviceProvider.GetRequiredService<IServiceProvider>().CreateScope())
            {
                // 1. 迁移 PersistentGrantDbContext 上下文
                scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

                // 2. 迁移 ConfigurationDbContext 上下文                
                var configurationContext = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                configurationContext.Database.Migrate();
                // 2.1 生成GetClients、GetIdentityResources、GetApiResources、GetApiScopes数据
                EnsureConfigurationData(configurationContext);

                // 3. 迁移 ApplicationDbContext 上下文
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                context.Database.Migrate();

                var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

                // 3.1 读取默认数据

                // 3.2 遍历用户

                // 3.3 遍历角色
                
            }

            Console.WriteLine("Done Seeding Database");
            Console.WriteLine();
        }


        /// <summary>
        /// 生成GetClients、GetIdentityResources、GetApiResources、GetApiScopes数据
        /// </summary>
        /// <param name="context"></param>
        public static void EnsureConfigurationData(ConfigurationDbContext context)
        {
            if (!context.Clients.Any())
            {
                Console.WriteLine("Clients Building");

                foreach (var client in Config.GetClients().ToList())
                {
                    context.Clients.Add(client.ToEntity());
                }
                context.SaveChanges();

                Console.WriteLine("Clients Generated successfully");
            }
            else
            {
                Console.WriteLine("Clients Already Exists");
            }

            if (!context.IdentityResources.Any())
            {
                Console.WriteLine("IdentityResources Building");

                foreach (var resource in Config.GetIdentityResources().ToList())
                {
                    context.IdentityResources.Add(resource.ToEntity());
                }

                context.SaveChanges();
                Console.WriteLine("IdentityResources Generated successfully");
            }
            else
            {
                Console.WriteLine("IdentityResources Already Exists");
            }


            if (!context.ApiResources.Any())
            {
                Console.WriteLine("ApiResources Building");
                foreach (var resource in Config.GetApiResources().ToList())
                {
                    context.ApiResources.Add(resource.ToEntity());
                }
                context.SaveChanges();
                Console.WriteLine("ApiResources Generated successfully");
            }
            else
            {
                Console.WriteLine("ApiResources Already Exists");
            }


            if (!context.ApiScopes.Any())
            {
                Console.WriteLine("ApiScopes Building");
                foreach (var resource in Config.GetApiScopes().ToList())
                {
                    context.ApiScopes.Add(resource.ToEntity());
                }
                context.SaveChanges();
                Console.WriteLine("ApiScopes Generated successfully");
            }
            else
            {
                Console.WriteLine("ApiScopes Already Exists");
            }
        }

    }
}
