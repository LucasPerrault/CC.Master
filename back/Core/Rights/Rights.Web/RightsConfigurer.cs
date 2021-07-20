using Lucca.Core.Rights;
using Lucca.Core.Rights.RightsHelper;
using Microsoft.Extensions.DependencyInjection;
using Remote.Infra.Extensions;
using Rights.Domain.Abstractions;
using Rights.Infra.Configuration;
using Rights.Infra.Remote;
using Rights.Infra.Services;
using Rights.Infra.Stores;

namespace Rights.Web
{
    public static class RightsConfigurer
    {
        public static void ConfigureServices(this IServiceCollection services, RightsConfiguration config)
        {

            services.AddSingleton(new UserPermissionsCache());

            services.AddHttpClient<DepartmentsRemoteService>((provider, client) =>
            {
                client.WithUserAgent(nameof(DepartmentsRemoteService))
                    .WithBaseAddress(config.ServerUri, config.DepartmentsEndpointPath)
                    .WithAuthScheme("Lucca").AuthenticateCurrentPrincipal(provider);
            });

            services.AddHttpClient<ApiKeyPermissionsRemoteService>((provider, client) =>
            {
                client.WithUserAgent(nameof(ApiKeyPermissionsRemoteService))
                    .WithBaseAddress(config.ServerUri, config.ForeignAppEndpointPath)
                    .WithAuthScheme("Lucca").AuthenticateCurrentPrincipal(provider);
            });

            services.AddHttpClient<UserPermissionsRemoteService>((provider, client) =>
            {
                client.WithUserAgent(nameof(UserPermissionsRemoteService))
                    .WithBaseAddress(config.ServerUri, config.UsersEndpointPath)
                    .WithAuthScheme("Lucca").AuthenticateCurrentPrincipal(provider);
            });

            services.AddScoped<ICloudControlPermissionsStore, CloudControlPermissionsStore>();

            services.AddScoped<ApiKeyPermissionsService>();
            services.AddScoped<UserPermissionsService>();

            services.AddScoped<ClaimsPrincipalRightsHelper, RightsHelper>();
            services.AddLuccaRights<PermissionsStore, ActorsStore, DepartmentsTreeStore>();
            services.AddScoped<IRightsService, RightsService>();
        }
    }
}
