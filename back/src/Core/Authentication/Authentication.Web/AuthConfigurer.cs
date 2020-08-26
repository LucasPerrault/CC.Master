using Authentication.Infra.Configurations;
using Authentication.Infra.Services;
using Authentication.Infra.Storage;
using Shared.Infra.Remote.Configurations;
using Lucca.Core.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Partenaires.Infra.Configuration;
using System;

namespace Authentication.Web
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
