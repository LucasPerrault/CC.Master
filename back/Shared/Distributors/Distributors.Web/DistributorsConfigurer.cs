using Distributors.Infra.Storage.Stores;
using Microsoft.Extensions.DependencyInjection;

namespace Distributors.Web
{
    public static class DistributorsConfigurer
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<DistributorsStore>();
            services.AddScoped<DistributorDomainsStore>();
        }
    }
}
