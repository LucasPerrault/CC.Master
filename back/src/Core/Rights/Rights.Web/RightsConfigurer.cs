using Lucca.Core.Rights;
using Lucca.Core.Rights.RightsHelper;
using Microsoft.Extensions.DependencyInjection;
using Rights.Domain.Abstractions;
using Rights.Infra.Services;
using Rights.Infra.Stores;

namespace Rights.Web
{
	public static class RightsConfigurer
	{
		public static void ConfigureServices(this IServiceCollection services)
		{
			services.AddScoped<ClaimsPrincipalRightsHelper, RightsHelper>();
			services.AddLuccaRights<PermissionsStore, ActorsStore, DepartmentsTreeStore>();
			services.AddScoped<IRightsService, RightsService>();
		}
	}
}
