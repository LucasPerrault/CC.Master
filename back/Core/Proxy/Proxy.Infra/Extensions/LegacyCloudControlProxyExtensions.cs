using Core.Proxy.Infra.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using ProxyKit;
using System;

namespace Core.Proxy.Infra.Extensions
{
	public static class LegacyCloudControlProxyExtensions
	{
		private const string FORWARDED_BY_LUCCA_HEADER = "X-Forwarded-By-Lucca";
		private const string FORWARDED_BY_CC_MASTER_HEADER = "X-Forwarded-By-CC-Master";
		private const string HOST_HEADER = "Host";

		private const string HTTP_REDIRECTION_ADDRESS = "http://127.0.0.1";
		private const string WS_REDIRECTION_ADDRESS = "ws://127.0.0.1";

		public static IApplicationBuilder UseLegacyCloudControlHttpProxy(this IApplicationBuilder app)
		{
			var proxyConfiguration = app.ApplicationServices.GetService<LegacyCloudControlConfiguration>();
			app.UseWhen
			(
				context  => context.IsRedirectableCall(),
				app => app.RunProxy(context => context
					.ForwardTo(HTTP_REDIRECTION_ADDRESS)
					.CopyXForwardedHeaders()
					.AddXForwardedHeaders()
					.AddRedirectionHeader(proxyConfiguration.Host)
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
				context => new Uri(WS_REDIRECTION_ADDRESS),
				options => options
					.AddRedirectionHeader(proxyConfiguration.Host)
					.AddXForwardedHeaders()
			);

			return app;
		}

		private static bool IsRedirectableCall(this HttpContext httpContext)
		{
			return true;
		}

		private static ForwardContext AddRedirectionHeader(this ForwardContext forwardContext, string redirectionUrl)
		{
			forwardContext.UpstreamRequest.Headers.Remove(HOST_HEADER);
			forwardContext.UpstreamRequest.Headers.Add(HOST_HEADER, new [] { redirectionUrl });
			return forwardContext;
		}

		private static WebSocketClientOptions AddRedirectionHeader(this WebSocketClientOptions options, string redirectionUrl)
		{
			options.HttpContext.Request.Headers.Remove(HOST_HEADER);
			options.HttpContext.Request.Headers.Add(HOST_HEADER, new [] { redirectionUrl });
			return options;
		}

		private static ForwardContext AddXForwardedCustomHeaders
			(this ForwardContext forwardContext, HttpContext context)
		{
			if (context.Request.Headers.TryGetValue(FORWARDED_BY_LUCCA_HEADER, out var forwardedByLuccaHeader))
			{
				forwardContext.UpstreamRequest.Headers.Add(FORWARDED_BY_LUCCA_HEADER, new [] { true.ToString()});
			}
			forwardContext.UpstreamRequest.Headers.Add(FORWARDED_BY_CC_MASTER_HEADER, new [] { context.Request.Host.Host });
			return forwardContext;
		}
	}
}
