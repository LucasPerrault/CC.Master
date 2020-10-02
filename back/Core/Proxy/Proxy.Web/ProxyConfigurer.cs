using Core.Proxy.Infra.Configuration;
using Core.Proxy.Infra.Healthz;
using Core.Proxy.Infra.Services;
using Microsoft.Extensions.DependencyInjection;
using ProxyKit;
using Remote.Infra.Configurations;
using System;

namespace Proxy.Web
{
	public static class ProxyConfigurer
	{
		private const int CloudControlTimeoutInSeconds = 120;
		public static void ConfigureServices(IServiceCollection services)
		{
			services.AddProxy(httpClientBuilder =>
				httpClientBuilder.ConfigureHttpClient(client => client.Timeout = TimeSpan.FromSeconds(CloudControlTimeoutInSeconds))
			);
		}

		public static void ConfigureLegacyHealthzServices(IServiceCollection services, LegacyCloudControlConfiguration config)
		{
			services.WithHostConfiguration(new LegacyCloudControlServiceConfiguration())
				.AddRemoteServiceHttpClient<LegacyHealthzService>(new Uri(config.HttpRedirectionUrl));
		}

		public static IHealthChecksBuilder AddLegacyCheck(this IHealthChecksBuilder builder)
		{
			builder.AddCheck<LegacyHealthCheck>("legacy_check");
			return builder;
		}
	}
}
