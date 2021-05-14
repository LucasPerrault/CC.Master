using Core.Proxy.Infra.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CloudControl.Web.Spa
{
    public static class FrontApplicationExtensions
    {
        public static void RegisterFrontApplication(this IServiceCollection services, IWebHostEnvironment env)
        {
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = env.GetFrontAppRootPath();
            });
        }

        public static IApplicationBuilder UseFrontStaticFiles(this IApplicationBuilder app)
        {
            app.UseSpaStaticFiles(new StaticFileOptions
            {
                RequestPath = "/cc-master",
            });

            return app;
        }

        public static IApplicationBuilder UseFrontApplication(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseWhen
            (
                c => !c.Request.Path.IsRootCall(),
                a => a.UseSpa(spa =>
                {
                    spa.Options.DefaultPage = "/index.html";
                    spa.Options.SourcePath = GetFrontAppRootPath(env);
                })
            );
            return app;
        }

        private static string GetFrontAppRootPath(this IWebHostEnvironment environment) =>
            environment.IsDevelopment()
                ? "../../front/cc-master/dist"
                : "./wwwroot";
    }
}
