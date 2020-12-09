using IpFilter.Domain;
using IpFilter.Infra;
using IpFilter.Infra.Storage.Stores;
using Lucca.Core.AspNetCore.Middlewares;
using Microsoft.Extensions.DependencyInjection;

namespace IpFilter.Web
{
    public static class IpFilterConfigurer
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IIpFilterService, IpFilterService>();
            services.AddScoped<IIpFilterAuthorizationStore, IpFilterAuthorizationStore>();
            services.AddLuccaIpWhitelist<CurrentUserIpAccessor>();
        }
    }
}
