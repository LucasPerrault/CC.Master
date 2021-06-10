using Billing.Cmrr.Application;
using Billing.Cmrr.Application.Interfaces;
using Billing.Cmrr.Domain.Interfaces;
using Billing.Cmrr.Infra.Storage.Stores;
using Billing.Contracts.Application.Clients;
using Billing.Contracts.Domain.Clients.Interfaces;
using Billing.Contracts.Domain.Contracts.Interfaces;
using Billing.Contracts.Infra.Clients.Stores;
using Billing.Contracts.Infra.Configurations;
using Billing.Contracts.Infra.Legacy;
using Billing.Contracts.Infra.Services;
using Billing.Contracts.Infra.Storage.Stores;
using Billing.Products.Domain.Interfaces;
using Billing.Products.Infra.Storage.Stores;
using Core.Proxy.Infra.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Remote.Infra.Extensions;

namespace Billing.Web
{
    public static class BillingConfigurer
    {
        public static void ConfigureServices(IServiceCollection services, LegacyCloudControlConfiguration legacyConfig, BillingContractsConfiguration config)
        {
            services.AddScoped<IClientsStore, ClientsStore>();
            services.AddScoped<IContractsStore, ContractsStore>();

            services.AddScoped<IClientVisibilityService, ClientVisibilityService>();

            services.AddHttpClient<ILegacyClientsRemoteService, LegacyClientsRemoteService>((provider, client) =>
            {
                client.WithUserAgent(nameof(LegacyClientsRemoteService))
                    .WithBaseAddress(legacyConfig.Uri, config.LegacyClientsEndpointPath)
                    .WithAuthScheme("CloudControl").AuthenticateCurrentPrincipal(provider);
            });

            services.AddScoped<ClientsRepository>();

            ConfigureCmrr(services);
            ConfigureProduct(services);
        }

        private static void ConfigureCmrr(IServiceCollection services)
        {
            services.AddScoped<ICmrrContractsStore, CmrrContractsStore>();
            services.AddScoped<ICmrrCountsStore, CmrrCountsStore>();
            services.AddScoped<IBreakdownService, BreakdownService>();

            services.AddScoped<ICmrrSituationsService, CmrrSituationsService>();
            services.AddScoped<IContractAxisSectionSituationsService, ContractAxisSectionSituationsService>();
        }

        private static void ConfigureProduct(IServiceCollection services)
        {
            services.AddScoped<IProductsStore, ProductsStore>();

        }
    }
}
