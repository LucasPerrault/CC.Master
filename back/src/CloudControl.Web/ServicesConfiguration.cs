using CloudControl.Shared.Infra.Configuration;
using CloudControl.Web.Exceptions;
using CloudControl.Web.Spa;
using Lucca.Logs.AspnetCore;
using Lucca.Logs.Shared;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProxyKit;
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
			ConfigureConfiguration(services);
			ConfigureLogs(services);
			ConfigureProxy(services);
			ConfigureSpa(services);
		}

		public virtual void ConfigureConfiguration(IServiceCollection services)
		{
			services.Configure<Configuration>(_configuration);

			var config = _configuration.Get<Configuration>();
			services.AddSingleton(config.LegacyCloudControl);
		}

		public virtual void ConfigureLogs(IServiceCollection services)
		{
			services
				.AddSingleton<IExceptionQualifier, CloudControlExceptionsQualifier>()
				.AddLogging(l =>
				{
					l.AddLuccaLogs(_configuration.GetSection(Configuration.LuccaLoggerOptionsKey), Configuration.AppName);
				});
		}

		public virtual void ConfigureProxy(IServiceCollection services)
		{
			services.AddProxy();
		}

		public virtual void ConfigureSpa(IServiceCollection services)
		{
			services.RegisterFrontApplication(_hostingEnvironment);
		}
	}
}
