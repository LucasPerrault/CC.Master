using AdvancedFilters.Domain.Billing.Interfaces;
using AdvancedFilters.Domain.Billing.Models;
using AdvancedFilters.Domain.Contacts.Interfaces;
using AdvancedFilters.Domain.Core.Collections;
using AdvancedFilters.Domain.DataSources;
using AdvancedFilters.Domain.Facets;
using AdvancedFilters.Domain.Instance.Interfaces;
using AdvancedFilters.Infra.Services;
using AdvancedFilters.Infra.Services.Sync;
using AdvancedFilters.Infra.Storage.Services;
using AdvancedFilters.Infra.Storage.Stores;
using AdvancedFilters.Web.Configuration;
using Lucca.Core.Api.Abstractions;
using Lucca.Core.Api.Web;
using Lucca.Core.PublicData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Remote.Infra.Extensions;
using Resources.Translations;
using System;
using AdvancedFilters.Application;
using Tools;
using System.Linq;
using System.Collections.Generic;

namespace AdvancedFilters.Web
{
    public static class AdvancedFiltersConfigurer
    {
        public static void ConfigureServices(IServiceCollection services, AdvancedFiltersConfiguration configuration)
        {
            services.AddSingleton(new DataSourcesRepository(DataSourceMapper.GetAll(configuration)));

            services.AddScoped<IAdvancedFiltersTranslations, AdvancedFiltersTranslations>();
            services.ConfigureStorage();
            services.ConfigureSync(configuration);

            services.ConfigureCore();
            services.ConfigureInstances();
            services.ConfigureBilling();
            services.ConfigureContacts();

            services.ConfigureFacets();

            services.AddScoped<IExportService, ExportCsvService>();
            services.AddScoped<IEnvironmentPopulator, EnvironmentPopulator>();
            services.AddScoped<IEstablishmentPopulator, EstablishmentPopulator>();
        }

        public static void ConfigureStorage(this IServiceCollection services)
        {
            services.AddScoped<IBulkUpsertService, BulkUpsertService>();
        }

        public static void ConfigureSync(this IServiceCollection services, AdvancedFiltersConfiguration configuration)
        {
            services.AddSingleton(new HttpConfiguration { MaxParallelCalls = configuration.MaxParallelHttpCalls });
            services.AddSingleton<FetchAuthenticator>();
            services.AddScoped<ILocalDataSourceService, LocalDataSourceService>();
            services.AddScoped<ISyncEmails, SyncEmails>();
            services.AddScoped<IDataSyncService, DataSyncService>();
            services.AddScoped<IFacetsSyncService, FacetsSyncService>();
            services.AddScoped<Synchronizer>();

            services.AddHttpClient<IDataSourceSyncCreationService, DataSourceSyncCreationService>
            (
                c => c.WithUserAgent("CC.Master - CAFE - DataSourceSynchronizer")
            );
        }

        private static void ConfigureCore(this IServiceCollection services)
        {
            services.AddSingleton<ICountriesCollection, CountriesCollection>();
            services.AddSingleton<IApplicationsCollection, ApplicationsCollection>();
        }

        private static void ConfigureInstances(this IServiceCollection services)
        {
            services.AddScoped<IEnvironmentsStore, EnvironmentsStore>();
            services.AddScoped<IAppInstancesStore, AppInstancesStore>();
        }

        private static void ConfigureBilling(this IServiceCollection services)
        {
            services.AddPublicData();

            services.AddScoped<IClientsStore, ClientsStore>();
            services.AddScoped<IDistributorsStore, DistributorsStore>();
            services.AddScoped<IContractsStore, ContractsStore>();
            services.AddScoped<ILegalUnitsStore, LegalUnitsStore>();
            services.AddScoped<IEstablishmentsStore, EstablishmentsStore>();
        }

        private static void ConfigureContacts(this IServiceCollection services)
        {
            services.AddScoped<IAppContactsStore, AppContactsStore>();
            services.AddScoped<IClientContactsStore, ClientContactsStore>();
            services.AddScoped<ISpecializedContactsStore, SpecializedContactsStore>();
        }

        private static void ConfigureFacets(this IServiceCollection services)
        {
            services.AddScoped<IFacetsStore, FacetsStore>();
        }

        public static LuccaApiBuilder ConfigureLuccaApiForAdvancedFilters(this LuccaApiBuilder luccaApiBuilder)
        {
            luccaApiBuilder
                .ConfigureSorting<Distributor>()
                .Allow(o => o.Id)
                .Allow(o => o.Name)
                .Allow(o => o.IsLucca);

            return luccaApiBuilder;
        }

        public static void ConfigureSerializer(JsonOptions jsonOptions)
        {

            var facetSerializers = new List<IPolymorphicSerializer>
            {
                Serializer.WithPolymorphism<IEnvironmentFacetValue, FacetType>(nameof(IEnvironmentFacetValue.Type))
                    .AddMatch<EnvironmentFacetValue<int>>(FacetType.Integer)
                    .AddMatch<EnvironmentFacetValue<string>>(FacetType.String)
                    .AddMatch<EnvironmentFacetValue<decimal>>(FacetType.Decimal)
                    .AddMatch<EnvironmentFacetValue<decimal>>(FacetType.Percentage)
                    .AddMatch<EnvironmentFacetValue<DateTime>>(FacetType.DateTime)
                    .Build(),
                Serializer.WithPolymorphism<IEstablishmentFacetValue, FacetType>(nameof(IEstablishmentFacetValue.Type))
                    .AddMatch<EstablishmentFacetValue<int>>(FacetType.Integer)
                    .AddMatch<EstablishmentFacetValue<string>>(FacetType.String)
                    .AddMatch<EstablishmentFacetValue<decimal>>(FacetType.Decimal)
                    .AddMatch<EstablishmentFacetValue<decimal>>(FacetType.Percentage)
                    .AddMatch<EstablishmentFacetValue<DateTime>>(FacetType.DateTime)
                    .Build(),
            };

            foreach (var converter in facetSerializers.SelectMany(s => s.GetConverters()))
            {
                jsonOptions.JsonSerializerOptions.Converters.Add(converter);
            }
        }
    }
}
