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
		private const string FORWARDED_BY_CC_MASTER_HEADER = "X-Forwarded-By-CC-Master";

		public static IApplicationBuilder UseLegacyCloudControlHttpProxy(this IApplicationBuilder app)
		{
			var proxyConfiguration = app.ApplicationServices.GetService<LegacyCloudControlConfiguration>();
			app.UseWhen
			(
				context  => context.IsRedirectableCall(),
				app => app.RunProxy(context => context
					.ForwardTo(proxyConfiguration.HttpRedirectionUrl)
					.CopyXForwardedHeaders()
					.AddXForwardedHeaders()
					.AddXForwardedCustomHeaders(context)
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
			return true;
		}

		private static ForwardContext AddXForwardedCustomHeaders(this ForwardContext forwardContext, HttpContext context)
		{
			forwardContext.UpstreamRequest.Headers.Add(FORWARDED_BY_CC_MASTER_HEADER, new [] { true.ToString()});
			return forwardContext;
		}
	}
}
