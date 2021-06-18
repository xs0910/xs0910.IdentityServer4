using IdentityServer4.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Reflection;
using xs0910.IdentityServer4.Authorization;
using xs0910.IdentityServer4.Models;

namespace xs0910.IdentityServer4
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IHostEnvironment Environment { get; }

        public Startup(IConfiguration configuration, IHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            services.AddSameSiteCookiePolicy();         // 自定义AddSameSiteCookiePolicy，不然 Context.User?.GetDisplayName() 取不到值
            services.AddMvc().AddRazorRuntimeCompilation();

            #region 数据库配置
            var isMySql = Configuration.GetConnectionString("IsMySql").ObjToBool();
            string connectionStrFile = Configuration.GetConnectionString("Connection_File");
            string connectionStr = string.Empty;
            if (File.Exists(connectionStrFile))
            {
                connectionStr = File.ReadAllText(connectionStrFile).Trim();
            }
            else
            {
                if (isMySql)
                {
                    connectionStr = Configuration.GetConnectionString("Connection_MySql");
                }
                else
                {
                    connectionStr = Configuration.GetConnectionString("Connection_MSSql");
                }
            }

            if (string.IsNullOrEmpty(connectionStr))
            {
                throw new Exception("数据库配置异常");
            }

            // 数据库配置系统应用用户数据上下文
            if (isMySql)
            {
                services.AddDbContext<ApplicationDbContext>(options => options.UseMySql(connectionStr, new MySqlServerVersion(new Version(8, 0, 23))));
            }
            else
            {
                services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionStr));
            }

            #endregion

            services.AddAutoMapper();           // 加入AutoMapper

            // 在IdentityServer4 服务前，需要启动 Identity服务，添加指定的用户和角色类型的默认标识系统配置
            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // 修改 Identity 应用配置
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = new PathString("/oauth2/authorize");
            });

            // 添加IdentityServer4 服务
            var builder = services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;

                // 自定义用户交互选项
                options.UserInteraction = new UserInteractionOptions()
                {
                    LoginUrl = "/oauth2/authorize",      //配置登录地址，因为我们已经改成/oauth2/authorize
                };
            })
            // 内存模式
            //.AddTestUsers(InMemoryConfig.Users().ToList())
            //.AddInMemoryIdentityResources(InMemoryConfig.GetIdentityResources())
            //.AddInMemoryApiScopes(InMemoryConfig.GetApiScopes())
            //.AddInMemoryApiResources(InMemoryConfig.GetApiResources())
            //.AddInMemoryClients(InMemoryConfig.GetClients())

            // 数据库模式
            .AddAspNetIdentity<ApplicationUser>()

            // 添加配置数据（客户端和资源）
            .AddConfigurationStore(options =>
            {
                if (isMySql)
                {
                    options.ConfigureDbContext = b => b.UseMySql(
                        connectionStr,
                        new MySqlServerVersion(new Version(8, 0, 23)),
                        sql => sql.MigrationsAssembly(migrationsAssembly)
                    );
                }
                else
                {
                    options.ConfigureDbContext = b => b.UseSqlServer(
                       connectionStr,
                       sql => sql.MigrationsAssembly(migrationsAssembly)
                   );
                }
            })

            // 添加操作数据
            .AddOperationalStore(options =>
            {
                if (isMySql)
                {
                    options.ConfigureDbContext = b => b.UseMySql(
                        connectionStr,
                        new MySqlServerVersion(new Version(8, 0, 23)),
                        sql => sql.MigrationsAssembly(migrationsAssembly)
                    );
                }
                else
                {
                    options.ConfigureDbContext = b => b.UseSqlServer(
                       connectionStr,
                       sql => sql.MigrationsAssembly(migrationsAssembly)
                   );
                }

                // this enables automic cleaup token. this is optional
                options.EnableTokenCleanup = true;
            })
            ;

            builder.AddDeveloperSigningCredential();

            services.AddAuthentication();       // 认证

            // 授权 添加授权策略
            services.AddAuthorization(options =>
            {
                options.AddPolicy("SuperAdmin", policy =>
                {
                    policy.Requirements.Add(new ClaimRequirement("role", "SuperAdmin"));
                });

                options.AddPolicy("Admin", policy =>
                {
                    policy.Requirements.Add(new ClaimRequirement("role", "SuperAdmin,SystemAdmin"));
                });
            });
            services.AddSingleton<IAuthorizationHandler, ClaimRequirementHandler>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCookiePolicy();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseStaticFiles();
            app.UseRouting();

            // 添加启动 IdentityServer 中间件
            app.UseIdentityServer();

            // 认证 授权 中间件
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
