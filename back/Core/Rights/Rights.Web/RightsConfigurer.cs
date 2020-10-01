using Lucca.Core.Rights;
using Lucca.Core.Rights.RightsHelper;
using Microsoft.Extensions.DependencyInjection;
using Partenaires.Infra.Configuration;
using Remote.Infra.Configurations;
using Rights.Domain.Abstractions;
using Rights.Infra.Configuration;
using Rights.Infra.Remote;
using Rights.Infra.Services;
using Rights.Infra.Stores;
using System;

namespace Rights.Web
{
	public static class RightsConfigurer
	{
		public static void ConfigureServices(this IServiceCollection services, RightsConfiguration config)
		{
			services.WithHostConfiguration(new PartenairesAuthServiceConfiguration())
				.AddRemoteServiceHttpClient<DepartmentsRemoteService>(new Uri(config.ServerUri, config.DepartmentsEndpointPath))
				.AddRemoteServiceHttpClient<ApiKeyPermissionsRemoteService>(new Uri(config.ServerUri, config.ForeignAppEndpointPath))
				.AddRemoteServiceHttpClient<UserPermissionsRemoteService>(new Uri(config.ServerUri, config.UsersEndpointPath));

			services.AddScoped<ApiKeyPermissionsService>();
			services.AddScoped<UserPermissionsService>();

			services.AddScoped<ClaimsPrincipalRightsHelper, RightsHelper>();
			services.AddLuccaRights<PermissionsStore, ActorsStore, DepartmentsTreeStore>();
			services.AddScoped<IRightsService, RightsService>();
		}
	}
}
