using Core.Proxy.Infra.Configuration;
using Core.Proxy.Infra.Services;
using Microsoft.Extensions.DependencyInjection;
using ProxyKit;
using Shared.Infra.Remote.Configurations;
using System;

namespace Proxy.Web
{
	public static class ProxyConfigurer
	{
		public static void ConfigureServices(IServiceCollection services)
		{
			services.AddProxy();
		}

		public static void ConfigureLegacyHealthzServices(IServiceCollection services, LegacyCloudControlConfiguration config)
		{
			services.WithHostConfiguration(new LegacyCloudControlServiceConfiguration())
				.AddRemoteServiceHttpClient<LegacyHealthzService>(new Uri(config.HttpRedirectionUrl));
		}
	}
}
