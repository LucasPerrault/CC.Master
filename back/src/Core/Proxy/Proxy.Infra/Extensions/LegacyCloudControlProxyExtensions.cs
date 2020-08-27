using Core.Proxy.Infra.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using ProxyKit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Proxy.Infra.Extensions
{
	public static class LegacyCloudControlProxyExtensions
	{
		private static readonly HashSet<string> _redirectableSegments = new HashSet<string>
		(
			StringComparer.InvariantCultureIgnoreCase
		)
		{
			"content",
			"signalR",
			"environments",
			"contractsv2",
			"errors"
		};

		private static readonly string _redirectableApiPrefix = "api/v3";

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
			if (httpContext == null || !httpContext.Request.Path.HasValue)
			{
				return false;
			}

			if (httpContext.Request.Path.StartsWithSegments(_redirectableApiPrefix))
			{
				return true;
			}

			var firstSegment = httpContext.Request.Path.Value.Split('/').First();
			return _redirectableSegments.Contains(firstSegment);
		}
	}
}
