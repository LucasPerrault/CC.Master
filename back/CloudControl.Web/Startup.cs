using Authentication.Web.Middlewares;
using CloudControl.Web.Middlewares;
using CloudControl.Web.Spa;
using Core.Proxy.Infra.Extensions;
using IpFilter.Web;
using Lucca.Core.AspNetCore.Healthz;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rights.Web;

namespace CloudControl.Web
{
    public class Startup
    {
        private readonly ServicesConfiguration _servicesConfiguration;

        public Startup(ServicesConfiguration servicesConfiguration)
        {
            _servicesConfiguration = servicesConfiguration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            _servicesConfiguration.ConfigureServices(services);
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHsts();

            app.UseHealthChecks();

            app.UseCertificateForwarding();
            app.UseRouting();

            app.UseMiddleware<SessionKeyAuthMiddleware>();
            app.UseAuthentication();
            app.UseMiddleware<BetaTesterDetectionMiddleware>();

            app.UseLegacyCloudControlWebSocketProxy();
            app.UseLegacyCloudControlHttpProxy();

            app.UseFrontStaticFiles();
            app.UseFileServer();

            app.UseMiddleware<UnauthorizedAccessMiddleware>();
            app.UseIpFilter(env);

            app.UseEndpoints(e => e.MapControllers());

            app.UseMiddleware<FrontRequestFilterMiddleware>();
            app.UseFrontApplication(env);
        }
    }
}
