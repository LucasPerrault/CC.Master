using Authentication.Infra.Configurations;
using CloudControl.Web;
using CloudControl.Web.Configuration;
using Lucca.Core.AspNetCore.Healthz;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rights.Infra.Configuration;
using System;

namespace CloudControl.Web.Tests.Mocks
{
	public class TestServicesConfiguration<TAuthenticationHandler> : ServicesConfiguration where TAuthenticationHandler : AuthenticationHandler<TestAuthenticationOptions>
	{
		public TestServicesConfiguration(IConfiguration configuration, IWebHostEnvironment env)
		: base(configuration, env)
		{ }

		public override AppConfiguration ConfigureConfiguration(IServiceCollection services) => null;

		public override void ConfigureLogs(IServiceCollection services)
		{
			services.AddLogging(l =>
			{
				l.AddConsole().AddDebug().SetMinimumLevel(LogLevel.Debug);
			});
		}

		public override void ConfigureAuthentication(IServiceCollection services, AppConfiguration appConfiguration)
		{
			base.ConfigureAuthentication(services, new AppConfiguration
			{
				Authentication = new AuthenticationConfiguration
				{
					ApiKeys = new ApiKeysConfiguration
					{
						new ApiKeyConfiguration { Id = 1, Name = "Mocked api key", Token = new Guid("deadbeef-0000-0000-0000-000000000000")}
					},
					ServerUri = new Uri("https://mocked-partenaires.local"),
					LogoutEndpointPath = "/logout",
					RedirectEndpointPath = "/login"
				},
			});
		}

		public override void ConfigureApi(IServiceCollection services)
		{
			services.AddControllers();
		}

		public override void ConfigureStorage(IServiceCollection services)
		{ }

		public override void ConfigureRights(IServiceCollection services, AppConfiguration configuration)
		{
			base.ConfigureRights(services, new AppConfiguration
			{
				Rights = new RightsConfiguration
				{
					ServerUri = new Uri("https://mocked-partenaires.local"),
					DepartmentsEndpointPath = "/api/mocked/Departments",
					UsersEndpointPath = "/api/mocked/Users",
					ForeignAppEndpointPath = "/api/mocked/ForeignApp"
				}
			});
		}

		public override void ConfigureEvents(IServiceCollection services, AppConfiguration configuration)
		{ }

		public override void ConfigureHealthCheck(IServiceCollection services, AppConfiguration c)
		{
			services
				.AddHealthCheck(o =>
					{
						o.ServiceGuid = new Guid("00000000-0000-0000-0000-000000000000");
						o.ServiceName = "MOCK";
					}
				);
		}
	}
}
