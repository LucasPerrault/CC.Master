using Authentication.Web;
using CloudControl.Web.Middlewares;
using CloudControl.Web.Spa;
using Core.Proxy.Infra.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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

			app.UseCertificateForwarding();
			app.UseRouting();

			app.UseHttpsRedirection();

			app.UseLegacyCloudControlWebSocketProxy();
			app.UseLegacyCloudControlHttpProxy();

			app.UseMiddleware<SessionKeyAuthMiddleware>();

			app.UseAuthentication();

			app.UseMiddleware<UnauthorizedAccessMiddleware>();

			app.UseMiddleware<FrontRequestFilterMiddleware>();
			app.UseFrontApplication(env);
		}
	}
}
