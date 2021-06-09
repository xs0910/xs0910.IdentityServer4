using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using xs0910.IdentityServer4.Models;
using xs0910.IdentityServer4.ViewModels;
using AutoMapper;
using System.Security.Claims;
using IdentityModel;

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
                Console.WriteLine();

                // 3. 迁移 ApplicationDbContext 上下文
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                context.Database.Migrate();

                var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

                // 3.1 读取默认数据
                var rootPath = scope.ServiceProvider.GetRequiredService<IHostEnvironment>().ContentRootPath;
                var dataPath = Path.Combine(rootPath, "Data\\Seed\\{0}.tsv");

                var users = JsonHelper.DeserializeObject<List<UserInfoDto>>(FileHelper.ReadFile(string.Format(dataPath, "UserInfo")));
                var roles = JsonHelper.DeserializeObject<List<RoleInfoDto>>(FileHelper.ReadFile(string.Format(dataPath, "RoleInfo")));
                var userRoles = JsonHelper.DeserializeObject<List<UserRoleInfoDto>>(FileHelper.ReadFile(string.Format(dataPath, "UserRoleInfo")));

                // 3.2 遍历用户
                var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
                Console.WriteLine("Creating Users");
                foreach (var user in users)
                {
                    if (user == null || string.IsNullOrEmpty(user.UserName))
                    {
                        continue;
                    }

                    var userItem = userMgr.FindByNameAsync(user.UserName).Result;       // 根据用户名查
                    var userRoleIds = userRoles.Where(r => r.UserId == user.ID).Select(r => r.RoleId).ToList();

                    if (userItem == null && userRoleIds.Count > 0)
                    {
                        userItem = mapper.Map<ApplicationUser>(user);

                        // AspNetUsers 表
                        var result = userMgr.CreateAsync(userItem, "Admin123456*").Result;
                        if (!result.Succeeded)
                        {
                            Console.WriteLine($"{userItem.UserName} Error Occurred:{result.Errors.First().Description}");
                            continue;
                        }

                        // AspNetUserClaims 表
                        var claims = new List<Claim>
                        {
                            new Claim(JwtClaimTypes.Name,userItem.UserName),
                            new Claim(JwtClaimTypes.Email,userItem.Email),
                        };
                        claims.AddRange(userRoleIds.Select(r => new Claim(JwtClaimTypes.Role, r.ToString())));

                        result = userMgr.AddClaimsAsync(userItem, claims).Result;

                        if (!result.Succeeded)
                        {
                            Console.WriteLine($"{userItem.UserName} Error Occurred:{result.Errors.First().Description}");
                            continue;
                        }

                        Console.WriteLine($"{userItem?.UserName} Created Sucessfully");
                    }
                    else
                    {
                        Console.WriteLine($"{userItem?.UserName} Already Exists");
                    }
                }
                Console.WriteLine("Create Users Completed");
                Console.WriteLine();

                // 3.3 遍历角色
                Console.WriteLine("Creating Roles");
                foreach (var role in roles)
                {
                    if (role == null || string.IsNullOrEmpty(role.Name))
                    {
                        continue;
                    }

                    var roleItem = roleMgr.FindByNameAsync(role.Name).Result;
                    if (roleItem == null)
                    {
                        roleItem = mapper.Map<ApplicationRole>(role);

                        // AspNetRoles 表
                        var result = roleMgr.CreateAsync(roleItem).Result;
                        if (!result.Succeeded)
                        {
                            Console.WriteLine($"{roleItem.Name} Error Occurred:{result.Errors.First().Description}");
                            continue;
                        }

                        Console.WriteLine($"{role.Name} Created Sucessfully");
                    }
                    else
                    {
                        Console.WriteLine($"{role?.Name} Already Exists");
                    }
                }
                Console.WriteLine("Create Roles Completed");
                Console.WriteLine();

                // 3.4 遍历用户角色管理表
                Console.WriteLine("Creating UserRoles Relation Table");
                foreach (var userRole in userRoles)
                {
                    SaveUserRole(context, userRole);
                }
                Console.WriteLine("Create UserRoles Relation Table Completed");
                Console.WriteLine();
            }

            Console.WriteLine("Done Seeding Database");
            Console.WriteLine();
        }


        /// <summary>
        /// 生成GetClients、GetIdentityResources、GetApiResources、GetApiScopes数据
        /// </summary>
        /// <param name="context"></param>
        private static void EnsureConfigurationData(ConfigurationDbContext context)
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


        private static void SaveUserRole(ApplicationDbContext context, UserRoleInfoDto userRole)
        {

            var entity = context.Find<ApplicationUserRole>(userRole.UserId, userRole.RoleId);

            if (entity == null)
            {
                entity = new ApplicationUserRole()
                {
                    UserId = userRole.UserId,
                    RoleId = userRole.RoleId
                };
                context.Add(entity);
                context.SaveChanges();
                Console.WriteLine($"UserId:{userRole.UserId} RoleId:{userRole.RoleId} Created Sucessfully");
            }
            else
            {
                Console.WriteLine($"UserId:{userRole.UserId} RoleId:{userRole.RoleId} Already Exists");
            }
        }
    }
}
