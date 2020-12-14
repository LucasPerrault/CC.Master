using Billing.Contracts.Application.Clients;
using Billing.Contracts.Domain.Clients.Interfaces;
using Billing.Contracts.Domain.Contracts.Interfaces;
using Billing.Contracts.Infra.Clients.Stores;
using Billing.Contracts.Infra.Configurations;
using Billing.Contracts.Infra.Legacy;
using Billing.Contracts.Infra.Services;
using Billing.Contracts.Infra.Storage.Stores;
using Core.Proxy.Infra.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Remote.Infra.Extensions;
using System;

namespace Billing.Web
{
	public static class BillingConfigurer
	{
		public static void ConfigureServices(IServiceCollection services, LegacyCloudControlConfiguration legacyConfig, BillingContractsConfiguration config)
		{
			services.AddScoped<IClientsStore, ClientsStore>();
			services.AddScoped<IContractsStore, ContractsStore>();

			services.AddScoped<IClientVisibilityService, ClientVisibilityService>();

			var legacyClientsUriBuilder = new UriBuilder
			{
				Host = legacyConfig.Host,
				Scheme = "https",
				Path = config.LegacyClientsEndpointPath
			};

			services.AddScoped<ILegacyClientsRemoteService, LegacyClientsRemoteService>();
			services.AddHttpClient<ILegacyClientsRemoteService, LegacyClientsRemoteService>((provider, client) =>
			{
				client.WithUserAgent(nameof(LegacyClientsRemoteService))
					.WithBaseAddress(legacyClientsUriBuilder.Uri)
					.WithAuthScheme("CloudControl").AuthenticateCurrentPrincipal(provider);
			});

			services.AddScoped<ClientsRepository>();
		}
	}
}
