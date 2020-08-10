using CloudControl.Shared.Infra.Authentication;
using CloudControl.Shared.Infra.Configuration;
using CloudControl.Web.Exceptions;
using CloudControl.Web.Spa;
using Lucca.Core.Authentication;
using Lucca.Logs.AspnetCore;
using Lucca.Logs.Shared;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProxyKit;
using System;
using System.Security.Claims;

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
			ConfigureAuthentication(services);
			ConfigureProxy(services);
			ConfigureSpa(services);
			ConfigureRemoteServices(services);
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

		public virtual void ConfigureAuthentication(IServiceCollection services)
		{
			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
			services.AddTransient<ClaimsPrincipal>
			(
				provider => provider.GetService<IHttpContextAccessor>().HttpContext.User
			);
			services.AddLuccaAuthentication<PrincipalStore>();
		}

		public virtual void ConfigureProxy(IServiceCollection services)
		{
			services.AddProxy();
		}

		public virtual void ConfigureSpa(IServiceCollection services)
		{
			services.RegisterFrontApplication(_hostingEnvironment);
		}

		public virtual void ConfigureRemoteServices(IServiceCollection services)
		{
			var config = _configuration.Get<Configuration>();
			InfraServicesConfigurer.ConfigureRemoteServices(services, config);
		}
	}
}
