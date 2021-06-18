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

            services.AddSameSiteCookiePolicy();         // �Զ���AddSameSiteCookiePolicy����Ȼ Context.User?.GetDisplayName() ȡ����ֵ
            services.AddMvc().AddRazorRuntimeCompilation();

            #region ���ݿ�����
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
                throw new Exception("���ݿ������쳣");
            }

            // ���ݿ�����ϵͳӦ���û�����������
            if (isMySql)
            {
                services.AddDbContext<ApplicationDbContext>(options => options.UseMySql(connectionStr, new MySqlServerVersion(new Version(8, 0, 23))));
            }
            else
            {
                services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionStr));
            }

            #endregion

            services.AddAutoMapper();           // ����AutoMapper

            // ��IdentityServer4 ����ǰ����Ҫ���� Identity�������ָ�����û��ͽ�ɫ���͵�Ĭ�ϱ�ʶϵͳ����
            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // �޸� Identity Ӧ������
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = new PathString("/oauth2/authorize");
            });

            // ���IdentityServer4 ����
            var builder = services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;

                // �Զ����û�����ѡ��
                options.UserInteraction = new UserInteractionOptions()
                {
                    LoginUrl = "/oauth2/authorize",      //���õ�¼��ַ����Ϊ�����Ѿ��ĳ�/oauth2/authorize
                };
            })
            // �ڴ�ģʽ
            //.AddTestUsers(InMemoryConfig.Users().ToList())
            //.AddInMemoryIdentityResources(InMemoryConfig.GetIdentityResources())
            //.AddInMemoryApiScopes(InMemoryConfig.GetApiScopes())
            //.AddInMemoryApiResources(InMemoryConfig.GetApiResources())
            //.AddInMemoryClients(InMemoryConfig.GetClients())

            // ���ݿ�ģʽ
            .AddAspNetIdentity<ApplicationUser>()

            // ����������ݣ��ͻ��˺���Դ��
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

            // ��Ӳ�������
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

            services.AddAuthentication();       // ��֤

            // ��Ȩ �����Ȩ����
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

            // ������� IdentityServer �м��
            app.UseIdentityServer();

            // ��֤ ��Ȩ �м��
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
