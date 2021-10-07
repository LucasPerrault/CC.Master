using AdvancedFilters.Domain.Billing.Interfaces;
using AdvancedFilters.Domain.Contacts.Interfaces;
using AdvancedFilters.Domain.Core.Collections;
using AdvancedFilters.Domain.DataSources;
using AdvancedFilters.Domain.Instance.Interfaces;
using AdvancedFilters.Infra.Services.Sync;
using AdvancedFilters.Infra.Storage.Services;
using AdvancedFilters.Infra.Storage.Stores;
using AdvancedFilters.Web.Configuration;
using Lucca.Core.PublicData;
using Microsoft.Extensions.DependencyInjection;
using Remote.Infra.Extensions;

namespace AdvancedFilters.Web
{
    public static class AdvancedFiltersConfigurer
    {
        public static void ConfigureServices(IServiceCollection services, AdvancedFiltersConfiguration configuration)
        {
            services.AddSingleton(new DataSourcesRepository(DataSourceMapper.GetAll(configuration)));

            services.ConfigureStorage();
            services.ConfigureSync(configuration);

            services.ConfigureCore();
            services.ConfigureInstances();
            services.ConfigureBilling();
            services.ConfigureContacts();
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
            services.AddScoped<SyncService>();

            services.AddHttpClient<IDataSourceSyncCreationService, DataSourceSyncCreationService>
            (
                c => c.WithUserAgent("CC.Master - CAFE - DataSourceSynchronizer")
            );
        }

        private static void ConfigureCore(this IServiceCollection services)
        {
            services.AddSingleton<ICountriesCollection, CountriesCollection>();
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
    }
}
