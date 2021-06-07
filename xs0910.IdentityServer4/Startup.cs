using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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
            services.AddMvc();

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
            #endregion

            // ���IdentityServer4 ����
            var builder = services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
            })
            .AddTestUsers(InMemoryConfig.Users().ToList())
            .AddInMemoryIdentityResources(InMemoryConfig.GetIdentityResources())
            .AddInMemoryApiScopes(InMemoryConfig.GetApiScopes())
            .AddInMemoryApiResources(InMemoryConfig.GetApiResources())
            .AddInMemoryClients(InMemoryConfig.GetClients())
            ;

            builder.AddDeveloperSigningCredential();

            services.AddAuthentication();       // ��֤
            services.AddAuthorization();        // ��Ȩ
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
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
