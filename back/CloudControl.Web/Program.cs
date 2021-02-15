using Lucca.Core.AspNetCore.EfMigration;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Proxy.Web;
using Storage.Infra.Migrations;
using System;
using System.Threading.Tasks;

namespace CloudControl.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await args.MigrateIfRequestedAsync<CloudControlMigrationStartup>();

            await CreateHostBuilder(args).Build().RunAsync();
        }

        private static IWebHostBuilder CreateHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args).ConfigureServices(s =>
                {
                    s.AddSingleton<ServicesConfiguration>();
                })
                .UseStartup<Startup>()
                .ConfigureKestrel(ConfigureKestrel);

        private static void ConfigureKestrel(KestrelServerOptions kestrelServerOptions)
        {
            kestrelServerOptions.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(ProxyConfigurer.CloudControlTimeoutInMinutes);
        }
    }
}
