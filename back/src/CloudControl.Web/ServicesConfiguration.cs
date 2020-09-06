using Authentication.Web;
using CloudControl.Web.Configuration;
using CloudControl.Web.Exceptions;
using CloudControl.Web.Spa;
using Distributors.Infra.Storage;
using Distributors.Web;
using Lucca.Core.AspNetCore.Healthz;
using Lucca.Logs.AspnetCore;
using Lucca.Logs.Shared;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Proxy.Web;
using Rights.Web;
using Storage.Infra.Context;
using Storage.Web;
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
			ConfigureHealthCheck(services, configuration);
			ConfigureLogs(services);
			ConfigureSpa(services);
			ConfigureProxy(services);
			ConfigureStorage(services);
			ConfigureSharedDomains(services);
			ConfigureAuthentication(services, configuration);
			ConfigureRights(services, configuration);
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

		public virtual void ConfigureHealthCheck(IServiceCollection services, AppConfiguration configuration)
		{
			ProxyConfigurer.ConfigureLegacyHealthzServices(services, configuration.LegacyCloudControl);

			services
				.AddHealthCheck(o =>
					{
						o.ServiceGuid = new Guid("101DFDBD-2438-43D1-9D22-63D1C46B3412");// TODO
						o.ServiceName = AppConfiguration.AppName;
					}
				)
				.AddLegacyCheck();
		}

		public virtual void ConfigureProxy(IServiceCollection services)
		{
			ProxyConfigurer.ConfigureServices(services);
		}

		public virtual void ConfigureStorage(IServiceCollection services)
		{
			StorageConfigurer.ConfigureServices(services, _configuration);
			services.ConfigureContext<DistributorsDbContext>(_hostingEnvironment);
		}

		public virtual void ConfigureSharedDomains(IServiceCollection services)
		{
			DistributorsConfigurer.ConfigureServices(services);
		}

		public virtual void ConfigureAuthentication(IServiceCollection services, AppConfiguration configuration)
		{
			AuthConfigurer.ConfigureServices(services, configuration.Authentication);
		}

		public virtual void ConfigureRights(IServiceCollection services, AppConfiguration configuration)
		{
			RightsConfigurer.ConfigureServices(services, configuration.Rights);
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
