using Billing.Contracts.Application;
using Billing.Contracts.Application.Clients;
using Billing.Contracts.Application.Offers;
using Billing.Contracts.Domain.Clients;
using Billing.Contracts.Domain.Clients.Interfaces;
using Billing.Contracts.Domain.Contracts;
using Billing.Contracts.Domain.Contracts.Health;
using Billing.Contracts.Domain.Contracts.Interfaces;
using Billing.Contracts.Domain.Counts.Interfaces;
using Billing.Contracts.Domain.Offers;
using Billing.Contracts.Domain.Offers.Filtering;
using Billing.Contracts.Domain.Offers.Interfaces;
using Billing.Contracts.Domain.Offers.Services;
using Billing.Contracts.Domain.Offers.Validation;
using Billing.Contracts.Infra.Configurations;
using Billing.Contracts.Infra.Legacy;
using Billing.Contracts.Infra.Storage.Stores;
using Core.Proxy.Infra.Configuration;
using Lucca.Core.Api.Abstractions;
using Lucca.Core.Api.Web;
using Microsoft.Extensions.DependencyInjection;
using Remote.Infra.Extensions;
using Resources.Translations;

namespace Billing.Contracts.Web
{
    public static class ContractsConfigurer
    {
        public static void ConfigureServices(IServiceCollection services, LegacyCloudControlConfiguration legacyConfig, BillingContractsConfiguration config)
        {
            services.AddSingleton<ContractHealthService>();

            services.AddScoped<IContractsStore, ContractsStore>();
            services.AddScoped<IContractEnvironmentStore, ContractEnvironmentStore>();
            services.AddScoped<ContractsRightsFilter>();
            services.AddScoped<ContractsRepository>();

            services.AddScoped<ICountsStore, CountsStore>();

            services.AddScoped<IClientsStore, ClientsStore>();
            services.AddScoped<ClientRightFilter>();
            services.AddScoped<ClientsRepository>();
            services.AddScoped<ContractHealthHelper>();

            services.AddScoped<ICommercialOfferUsageService, CommercialOfferUsageService>();
            services.AddScoped<CommercialOfferValidationService>();
            services.AddScoped<ICommercialOffersStore, CommercialOffersStore>();
            services.AddScoped<CommercialOffersRepository>();
            services.AddScoped<ITranslations, Translations>();
            services.AddScoped<CommercialOfferRightsFilter>();
            services.AddScoped<IImportedOffersService, ImportedOffersService>();

            services.AddHttpClient<ILegacyClientsRemoteService, LegacyClientsRemoteService>((provider, client) =>
            {
                client.WithUserAgent(nameof(LegacyClientsRemoteService))
                    .WithBaseAddress(legacyConfig.Uri, config.LegacyClientsEndpointPath)
                    .WithAuthScheme("CloudControl").AuthenticateCurrentPrincipal(provider);
            });
        }

        public static LuccaApiBuilder ConfigureLuccaApiForContracts(this LuccaApiBuilder luccaApiBuilder)
        {
            luccaApiBuilder
                .ConfigureSorting<CommercialOffer>()
                .Allow(o => o.Id)
                .Allow(o => o.Name)
                .Allow(o => o.ProductId)
                .Allow(o => o.BillingMode)
                .Allow(o => o.ForecastMethod)
                .Allow(o => o.PricingMethod)
                .Allow(o => o.CurrencyId);

            return luccaApiBuilder;
        }
    }
}
