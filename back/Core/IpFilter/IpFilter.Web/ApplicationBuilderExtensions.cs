using IpFilter.Domain;
using Lucca.Core.AspNetCore.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Hosting;
using System.Linq;

namespace IpFilter.Web
{
    public static class ApplicationBuilderExtensions
    {
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
            return HasAttribute<AllowAllIpsAttribute>(httpContext);
        }

        private static bool HasAttribute<T>(HttpContext context)
        {
            var endpoint = context.Features?
                .Get<IEndpointFeature>()?
                .Endpoint;

            return endpoint != null && endpoint.Metadata.Any(m => m is T);
        }
    }
}
