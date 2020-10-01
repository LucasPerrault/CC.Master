using Authentication.Domain;
using Authentication.Infra.Configurations;
using Authentication.Infra.Services;
using Authentication.Infra.Storage;
using Lucca.Core.Authentication;
using Lucca.Core.Authentication.Abstractions.Tokens;
using Lucca.Core.Authentication.Tokens;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Partenaires.Infra.Configuration;
using Remote.Infra.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;

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

			ConfigureCustomTokensReaders(services);

			services.AddSingleton<AuthRedirectionRemoteService>();

			services.WithHostConfiguration(new PartenairesAuthServiceConfiguration())
				.AddRemoteServiceHttpClient<UserAuthenticationRemoteService>(new Uri(config.ServerUri, config.UsersEndpointPath));

			var authApiKey = new ApiKey { Name = "Api keys fetcher" , Token = config.ApiKeysFetcherToken };
			services.WithHostConfiguration(new ApiKeyPartenairesServiceConfiguration(authApiKey))
				.AddRemoteServiceHttpClient<ApiKeyAuthenticationRemoteService>(new Uri(config.ServerUri, config.ApiKeysEndpointPath));

			services.AddSingleton<PrincipalStore>();
			services.AddSingleton<SessionKeyService>();
			services.AddSingleton<AuthTokenCookieService>();
		}

		private static void ConfigureCustomTokensReaders(IServiceCollection services)
		{
			var provider = services.BuildServiceProvider();
			var standardReaders = provider.GetRequiredService<IEnumerable<ISpecializedTokensReader>>();
			var customReaders = standardReaders
				.Where(r => !(r is HeadersTokensReader))
				.Union(new List<ISpecializedTokensReader> { new CloudControlHeaderTokensReader() });

			services.RemoveAll(typeof(ISpecializedTokensReader));
			services.TryAddEnumerable(customReaders.Select(r => new ServiceDescriptor(typeof(ISpecializedTokensReader), r)));
		}
	}
}
