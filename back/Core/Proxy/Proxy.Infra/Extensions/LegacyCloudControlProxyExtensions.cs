using Core.Proxy.Infra.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using ProxyKit;
using Rights.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using Tools.Web;

namespace Core.Proxy.Infra.Extensions
{
    public static class LegacyCloudControlProxyExtensions
    {
        private const string FORWARDED_BY_LUCCA_HEADER = "X-Forwarded-By-Lucca";
        private const string FORWARDED_BY_CC_MASTER_HEADER = "X-Forwarded-By-CC-Master";
        private const string HOST_HEADER = "Host";

        private const string HTTP_REDIRECTION_ADDRESS = "http://127.0.0.1";
        private const string WS_REDIRECTION_ADDRESS = "ws://127.0.0.1";

        private static readonly HashSet<string> NonV3LegacyApiSegments = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "workerprocesses",
            "backoffice",
        };
        private static readonly HashSet<string> NonRedirectableSegments = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "/account/login",
            "/logout",
            "/ping",
            "/healthz",
            "/health/ready",
            "/health/live",
            "/warmup",
            "/ip-filter",

            // front
            "/cc-master",
            "/logs",
            "/sources",
            "/accounting",
            "/reports",
            "/offers",
            "/contracts",
            "/ip",
            "/invalid-email-domain",
            "/demos"
        };

        private static readonly HashSet<string> BetaNonRedirectableSegments = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        { };

        public static IApplicationBuilder UseLegacyCloudControlHttpProxy(this IApplicationBuilder app)
        {
            var proxyConfiguration = app.ApplicationServices.GetService<LegacyCloudControlConfiguration>();
            app.UseWhen
                (
                    context => context.ShouldRedirect(),
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

        internal static bool ShouldRedirect(this HttpContext httpContext)
        {
            if (httpContext.Request.Path.IsRootCall())
            {
                return true;
            }

            if (httpContext.Request.Path.IsKnownNonRedirectableSegment())
            {
                return false;
            }

            if (BetaTesterHelper.IsBetaTester(httpContext) && httpContext.Request.Path.IsBetaKnownNonRedirectableSegment())
            {
                return false;
            }

            return httpContext.Request.Path.StartsWithSegments("/api/v3")
                   || httpContext.Request.IsNonV3LegacyApiPath()
                   || !httpContext.Request.IsApiCall();
        }

        public static bool IsRootCall(this PathString pathString)
        {
            return !pathString.HasValue || pathString == "/";
        }

        private static bool IsKnownNonRedirectableSegment(this PathString pathString)
        {
            return NonRedirectableSegments.Any(s => pathString.StartsWithSegments(s));
        }

        private static bool IsBetaKnownNonRedirectableSegment(this PathString pathString)
        {
            return BetaNonRedirectableSegments.Any(s => pathString.StartsWithSegments(s));
        }

        private static bool IsNonV3LegacyApiPath(this HttpRequest request)
        {
            if (!request.Path.HasValue)
            {
                return false;
            }

            if (!request.IsApiCall())
            {
                return false;
            }

            var secondSegment = request.Path.Value.Split('/').Skip(2).FirstOrDefault();
            return !string.IsNullOrEmpty(secondSegment) && NonV3LegacyApiSegments.Contains(secondSegment);
        }

        private static ForwardContext AddRedirectionHeader(this ForwardContext forwardContext, string redirectionUrl)
        {
            forwardContext.UpstreamRequest.Headers.Remove(HOST_HEADER);
            forwardContext.UpstreamRequest.Headers.Add(HOST_HEADER, new[] { redirectionUrl });
            return forwardContext;
        }

        private static WebSocketClientOptions AddRedirectionHeader(this WebSocketClientOptions options, string redirectionUrl)
        {
            options.HttpContext.Request.Headers.Remove(HOST_HEADER);
            options.HttpContext.Request.Headers.Add(HOST_HEADER, new[] { redirectionUrl });
            return options;
        }

        internal static ForwardContext AddXForwardedCustomHeaders(this ForwardContext forwardContext, HttpContext context)
        {
            if (context.Request.Headers.TryGetValue(FORWARDED_BY_LUCCA_HEADER, out var forwardedByLuccaHeader))
            {
                forwardContext.UpstreamRequest.Headers.Add(FORWARDED_BY_LUCCA_HEADER, new[] { true.ToString() });
            }
            forwardContext.UpstreamRequest.Headers.Add(FORWARDED_BY_CC_MASTER_HEADER, new[] { context.Request.Host.Host });
            return forwardContext;
        }
    }
}
