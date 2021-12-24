using Distributors.Application;
using Distributors.Domain;
using Distributors.Infra.Storage.Stores;
using Microsoft.Extensions.DependencyInjection;

namespace Distributors.Web
{
    public static class DistributorsConfigurer
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<DistributorsCache>();
            services.AddSingleton<DistributorDomainsCache>();

            services.AddScoped<IDistributorsStore, DistributorsStore>();
            services.AddScoped<IDistributorDomainsStore, DistributorDomainsStore>();

            services.AddScoped<DistributorsRepository>();
            services.AddScoped<IDistributorDomainService, DistributorDomainService>();
        }
    }
}
