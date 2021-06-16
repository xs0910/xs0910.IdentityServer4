using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using xs0910.IdentityServer4.Data;
using Serilog;
using Serilog.AspNetCore;
using Serilog.Sinks.SystemConsole.Themes;
using Serilog.Events;

namespace xs0910.IdentityServer4
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = "IdentityServer4 认证中心";

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .MinimumLevel.Override("System", LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.File(@"Logs/IdentityServer4_log.txt")
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}", theme: AnsiConsoleTheme.Literate)
                .CreateLogger();

            try
            {
                Log.Information("Starting IdentityServer4");
                var seed = args.Contains("/seed");
                if (seed)
                {
                    args = args.Except(new[] { "/seed" }).ToArray();
                }

                var host = CreateHostBuilder(args).Build();

                if (seed)
                {
                    SeedData.EnsureSeedData(host.Services);
                }

                host.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "IdentityServer4 Terminated UnExpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureLogging((hostingContext, builder) =>
                {
                    builder.ClearProviders();
                    builder.SetMinimumLevel(LogLevel.Information);
                    builder.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    builder.AddConsole();
                    builder.AddDebug();
                })
                .UseSerilog(dispose: true);
    }
}
