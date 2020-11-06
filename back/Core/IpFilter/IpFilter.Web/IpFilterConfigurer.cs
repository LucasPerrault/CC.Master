using IpFilter.Domain;
using IpFilter.Infra;
using Microsoft.Extensions.DependencyInjection;

namespace IpFilter.Web
{
    public static class IpFilterConfigurer
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IIpFilterService, IpFilterService>();
            services.AddSingleton<IIpFilterAuthorizationStore, IIpFilterAuthorizationStore>();
        }
    }
}
