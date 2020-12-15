using IpFilter.Domain;
using Lucca.Core.AspNetCore.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using Tools.Web;

namespace IpFilter.Web
{
    public static class ApplicationBuilderExtensions
    {
        private static HashSet<string> WhitelistedRoutes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "/ping",
            "/healthz",
            "/health/ready",
            "/health/live",
            "/warmup"
        };

        public static void UseIpFilter(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsProduction())
            {
                var forwardOpts = new ForwardedHeadersOptions
                {
                    ForwardedHeaders = ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedFor
                };
                forwardOpts.KnownNetworks.Clear();
                forwardOpts.KnownProxies.Clear();
                app.UseForwardedHeaders(forwardOpts);
            }

            app.UseWhen(c => !c.IsAccessibleForAllIps(), app => app.UseIpWhitelistMiddleware());
        }

        internal static bool IsAccessibleForAllIps(this HttpContext httpContext)
        {
            return WhitelistedRoutes.Contains(httpContext.Request.Path)
                || httpContext.HasAttribute<AllowAllIpsAttribute>();
        }
    }
}
