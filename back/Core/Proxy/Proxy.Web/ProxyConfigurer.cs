﻿using Core.Proxy.Infra.Configuration;
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
		public const int CloudControlTimeoutInMinutes = 15;
		public static void ConfigureServices(IServiceCollection services)
		{
			services.AddProxy(httpClientBuilder =>
				httpClientBuilder.ConfigureHttpClient(client => client.Timeout = TimeSpan.FromMinutes(CloudControlTimeoutInMinutes))
			);
		}

		public static void ConfigureLegacyHealthzServices(IServiceCollection services, LegacyCloudControlConfiguration config)
		{
			var uri = new UriBuilder { Host = config.Host, Scheme = "http"}.Uri;

			services.AddSingleton<LegacyHealthzService>();
			services.AddHttpClient<LegacyHealthzService>(client =>
			{
				client.WithUserAgent(nameof(LegacyHealthzService))
					.WithBaseAddress(uri);
			});
		}

		public static IHealthChecksBuilder AddLegacyCheck(this IHealthChecksBuilder builder)
		{
			builder.AddCheck<LegacyHealthCheck>("legacy_check");
			return builder;
		}
	}
}
