using CloudControl.Shared.Infra.Proxy.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using ProxyKit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CloudControl.Web.Proxy
{
	public static class LegacyCloudControlProxyExtensions
	{
		private static readonly List<string> _redirectableApiPrefixes = new List<string>
		{
			"/api/v3",
			"/ping",
			"/content",
			"/signalR",
			"/environments",
			"/contractsv2",
			"/errors"
		};

		public static IServiceCollection AddProxy(IServiceCollection services)
		{
			return services.AddProxy();
		}

		public static IApplicationBuilder UseLegacyCloudControlHttpProxy(this IApplicationBuilder app)
		{
			var proxyConfiguration = app.ApplicationServices.GetService<LegacyCloudControlConfiguration>();
			app.UseWhen
			(
				context  => context.IsRedirectableCall(),
				app => app.RunProxy(context => context
					.ForwardTo(proxyConfiguration.HttpRedirectionUrl)
					.AddXForwardedHeaders()
					.Send()
			));

			return app;
		}

		public static IApplicationBuilder UseLegacyCloudControlWebSocketProxy(this IApplicationBuilder app)
		{
			var proxyConfiguration = app.ApplicationServices.GetService<LegacyCloudControlConfiguration>();
			app.UseWebSockets();
			app.UseWebSocketProxy
			(
				context => new Uri(proxyConfiguration.WebSocketRedirectionUrl),
				options => options.AddXForwardedHeaders()
			);

			return app;
		}

		private static bool IsRedirectableCall(this HttpContext httpContext)
		{
			return httpContext != null &&
					httpContext.Request.Path.HasValue &&
					_redirectableApiPrefixes.Any(prefix =>
						httpContext.Request.Path.StartsWithSegments(prefix, StringComparison.OrdinalIgnoreCase));
		}
	}
}
