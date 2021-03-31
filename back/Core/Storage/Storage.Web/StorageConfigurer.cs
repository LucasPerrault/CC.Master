using Lucca.Core.AspNetCore.Tenancy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Storage.Infra.Migrations;

namespace Storage.Web
{
    public static class StorageConfigurer
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<TenantConnectionStringProvider>();
            SqlConfigurer.Configure(services, configuration);
        }
    }
}
