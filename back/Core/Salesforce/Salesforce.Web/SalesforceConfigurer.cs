using Microsoft.Extensions.DependencyInjection;
using Remote.Infra.Configurations;
using Salesforce.Domain.Interfaces;
using Salesforce.Infra.Configurations;
using Salesforce.Infra.Services;
using System;

namespace Salesforce.Web
{
    public static class SalesforceConfigurer
	{
		public static void ConfigureServices(IServiceCollection services, SalesforceConfiguration config)
		{
			services.WithHostConfiguration(new SalesforceServiceConfiguration(config.Token))
				.AddRemoteServiceHttpClient<ISalesforceAccountsRemoteService, SalesforceAccountsRemoteService>(new Uri(config.ServerUri, config.AccountsEndpointPath));
		}
	}
}
