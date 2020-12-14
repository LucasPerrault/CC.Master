using Microsoft.Extensions.DependencyInjection;
using Remote.Infra.Extensions;
using Salesforce.Domain.Interfaces;
using Salesforce.Infra.Configurations;
using Salesforce.Infra.Services;

namespace Salesforce.Web
{
    public static class SalesforceConfigurer
	{
		public static void ConfigureServices(IServiceCollection services, SalesforceConfiguration config)
		{
			services.AddScoped<ISalesforceAccountsRemoteService, SalesforceAccountsRemoteService>();
			services.AddHttpClient<ISalesforceAccountsRemoteService, SalesforceAccountsRemoteService>(client =>
			{
				client.WithUserAgent(nameof(SalesforceAccountsRemoteService))
					.WithBaseAddress(config.ServerUri, config.AccountsEndpointPath)
					.WithAuthScheme("Bearer").Authenticate(config.Token);
			});
		}
	}
}
