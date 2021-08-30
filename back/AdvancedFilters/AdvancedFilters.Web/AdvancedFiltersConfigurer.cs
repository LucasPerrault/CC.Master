using AdvancedFilters.Domain.Billing.Interfaces;
using AdvancedFilters.Domain.Contacts.Interfaces;
using AdvancedFilters.Domain.DataSources;
using AdvancedFilters.Domain.Instance.Interfaces;
using AdvancedFilters.Infra.Services;
using AdvancedFilters.Infra.Storage.Services;
using AdvancedFilters.Infra.Storage.Stores;
using AdvancedFilters.Web.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AdvancedFilters.Web
{
    public static class AdvancedFiltersConfigurer
    {
        public static void ConfigureServices(IServiceCollection services, AdvancedFiltersConfiguration configuration)
        {
            services.AddSingleton(new DataSourcesRepository(DataSourceMapper.GetAll(configuration)));

            services.ConfigureStorage();
            services.ConfigureSync();

            services.ConfigureInstances();
            services.ConfigureBilling();
            services.ConfigureContacts();
        }

        public static void ConfigureStorage(this IServiceCollection services)
        {
            services.AddScoped<BulkUpsertService>();
        }

        public static void ConfigureSync(this IServiceCollection services)
        {
            services.AddSingleton<FetchAuthenticator>();
            services.AddHttpClient<IDataSourceSynchronizerBuilder, DataSourceSynchronizerBuilder>();
            services.AddScoped<HugeSyncService>();
        }

        private static void ConfigureInstances(this IServiceCollection services)
        {
            services.AddScoped<IEnvironmentsStore, EnvironmentsStore>();
            services.AddScoped<IAppInstancesStore, AppInstancesStore>();
        }

        private static void ConfigureBilling(this IServiceCollection services)
        {
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
