using Lucca.Core.AspNetCore.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Hosting;

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

            app.UseIpWhitelistMiddleware();
        }
    }
}
