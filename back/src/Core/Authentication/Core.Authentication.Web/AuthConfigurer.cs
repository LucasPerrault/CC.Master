using CloudControl.Shared.Infra.Remote.Configurations;
using Core.Authentication.Infra.Configurations;
using Core.Authentication.Infra.Services;
using Core.Authentication.Infra.Storage;
using Lucca.Core.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Core.Authentication.Web
{
	public class AuthConfigurer
	{
		public static void ConfigureServices(IServiceCollection services, AuthenticationConfiguration config)
		{
			services.AddSingleton(config);
			services.AddTransient
			(
				provider => provider.GetService<IHttpContextAccessor>().HttpContext.User
			);
			services.AddLuccaAuthentication<PrincipalStore>();

			services.AddSingleton<AuthRedirectionRemoteService>();

			services.WithHostConfiguration(new PartenairesAuthServiceConfiguration())
				.AddRemoteServiceHttpClient<AuthenticationRemoteService>(new Uri(config.ServerUri, config.EndpointPath));

			services.AddSingleton<PrincipalStore>();
		}
	}
}
