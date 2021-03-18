using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;

namespace CloudControl.Web.Spa
{
    public static class FrontApplicationExtensions
    {
        private const string spaStaticPath = "/static";

        public static void RegisterFrontApplication(this IServiceCollection services, IWebHostEnvironment env)
        {
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = env.GetFrontAppRootPath();
            });
        }

        public static IApplicationBuilder UseFrontApplication(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseWhen
                (
                    context => context.IsStaticCall(),
                    app => app.UseSpaStaticFiles(new StaticFileOptions
                    {
                        FileProvider = new PhysicalFileProvider
                            (Path.Combine(env.ContentRootPath, GetFrontAppRootPath(env))),
                        RequestPath = spaStaticPath
                    })
                );

            app.UseWhen
                (
                    context => context.IsSpaCall(),
                    app => app.UseSpa(spa =>
                    {
                        spa.Options.DefaultPage = "/index.html";
                        spa.Options.SourcePath = env.GetFrontAppRootPath();
                    })
                );

            return app;
        }

        private static bool IsStaticCall(this HttpContext httpContext)
        {
            return httpContext != null &&
                   httpContext.Request.Path.HasValue &&
                   httpContext.Request.Path.Value.StartsWith(spaStaticPath, StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsSpaCall(this HttpContext httpContext)
        {
            return !httpContext.IsStaticCall();
        }

        private static string GetFrontAppRootPath(this IWebHostEnvironment environment) =>
            environment.IsDevelopment()
                ? "../../../front/dist"
                : "./wwwroot";
    }
}
