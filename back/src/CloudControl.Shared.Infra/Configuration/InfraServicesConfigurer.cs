using CloudControl.Shared.Infra.Authentication;
using CloudControl.Shared.Infra.Authentication.Configurations;
using CloudControl.Shared.Infra.Remote.Configurations;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;

namespace CloudControl.Shared.Infra.Configuration
{
    public static class InfraServicesConfigurer
	{
		public static void ConfigureRemoteServices(IServiceCollection services, Configuration config)
        {
            services.AddSingleton<JsonSerializer>();
            ConfigureConfiguration(services, config);

            services.WithHostConfiguration(new PartenairesAuthServiceConfiguration())
                .AddRemoteServiceHttpClient<AuthenticationRemoteService>(new Uri(config.Authentication.ServerUri, config.Authentication.EndpointPath));

            services.AddSingleton<PrincipalStore>();
        }

        private static void ConfigureConfiguration(IServiceCollection services, Configuration config)
        {
            services.AddSingleton(config.Authentication);
            services.AddSingleton(config.ApiKeys);
        }
    }
}
