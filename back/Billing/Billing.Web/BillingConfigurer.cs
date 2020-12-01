using Billing.Contracts.Domain.Clients.Interfaces;
using Billing.Contracts.Domain.Contracts.Interfaces;
using Billing.Contracts.Infra.Clients.Stores;
using Billing.Contracts.Infra.Storage.Stores;
using Microsoft.Extensions.DependencyInjection;

namespace Billing.Web
{
	public static class BillingConfigurer
	{
		public static void ConfigureServices(IServiceCollection services)
		{
			services.AddScoped<IClientsStore, ClientsStore>();
			services.AddScoped<IContractsStore, ContractsStore>();

			services.AddScoped<IClientVisibilityService, ClientVisibilityService>();
		}
	}
}
