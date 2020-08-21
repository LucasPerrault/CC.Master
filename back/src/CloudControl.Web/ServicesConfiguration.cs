using CloudControl.Web.Configuration;
using CloudControl.Web.Exceptions;
using CloudControl.Web.Spa;
using Core.Authentication.Web;
using Core.Proxy.Web;
using Lucca.Logs.AspnetCore;
using Lucca.Logs.Shared;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;

namespace CloudControl.Web
{
	public class ServicesConfiguration
	{
		private IConfiguration _configuration { get; }
		private IWebHostEnvironment _hostingEnvironment { get; }

		public ServicesConfiguration(IConfiguration configuration, IWebHostEnvironment env)
		{
			_configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
			_hostingEnvironment = env ?? throw new ArgumentNullException(nameof(env));
		}

		public void ConfigureServices(IServiceCollection services)
		{
			var configuration = ConfigureConfiguration(services);
			ConfigureHttpContext(services);
			ConfigureLogs(services);
			ConfigureSpa(services);
			ConfigureProxy(services);
			ConfigureAuthentication(services, configuration);
		}

		public virtual AppConfiguration ConfigureConfiguration(IServiceCollection services)
		{
			services.Configure<AppConfiguration>(_configuration);

			var config = _configuration.Get<AppConfiguration>();
			services.AddSingleton(config.LegacyCloudControl);

			return config;
		}

		public virtual void ConfigureHttpContext(IServiceCollection services)
		{
			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
			services.AddSingleton<JsonSerializer>();
		}

		public virtual void ConfigureProxy(IServiceCollection services)
		{
			ProxyConfigurer.ConfigureServices(services);
		}

		public virtual void ConfigureAuthentication(IServiceCollection services, AppConfiguration configuration)
		{
			AuthConfigurer.ConfigureServices(services, configuration.Authentication);
		}

		public virtual void ConfigureLogs(IServiceCollection services)
		{
			services
				.AddSingleton<IExceptionQualifier, CloudControlExceptionsQualifier>()
				.AddLogging(l =>
				{
					l.AddLuccaLogs(_configuration.GetSection(AppConfiguration.LuccaLoggerOptionsKey), AppConfiguration.AppName);
				});
		}

		public virtual void ConfigureSpa(IServiceCollection services)
		{
			services.RegisterFrontApplication(_hostingEnvironment);
		}
	}
}
