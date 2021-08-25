using AdvancedFilters.Domain.Billing.Interfaces;
using AdvancedFilters.Domain.Contacts.Interfaces;
using AdvancedFilters.Domain.DataSources;
using AdvancedFilters.Domain.Instance;
using AdvancedFilters.Domain.Instance.Interfaces;
using AdvancedFilters.Infra.Storage.Stores;
using AdvancedFilters.Web.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace AdvancedFilters.Web
{
    public class AdvancedFiltersConfiguration
    {
        public RoutesConfiguration Routes { get; set; }
        public AuthenticationConfiguration Auth { get; set; }
    }

    public static class AdvancedFiltersConfigurer
    {
        public static void ConfigureServices(IServiceCollection services, AdvancedFiltersConfiguration configuration)
        {
            services.AddSingleton(new DataSourcesRepository(DataSourceMapper.GetAll(configuration)));

            services.ConfigureInstances();
            services.ConfigureBilling();
            services.ConfigureContacts();
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

    public static class DataSourceMapper
    {
        public static Dictionary<DataSources, DataSource> GetAll(AdvancedFiltersConfiguration configuration)
        {

            return Enum.GetValues(typeof(DataSources)).Cast<DataSources>().ToDictionary(s => s, s => Get(s, configuration));
        }

        private static DataSource Get(DataSources source, AdvancedFiltersConfiguration configuration) => source switch
        {

            DataSources.Environments => configuration.Environment(),
            DataSources.Establishments => configuration.Establishment(),
            DataSources.AppInstances => configuration.AppInstance(),
            DataSources.LegalUnit => configuration.LegalUnit(),
            _ => throw new InvalidEnumArgumentException(nameof(source), (int)source, typeof(DataSources))
        };
    }

    internal static class AdvancedFiltersConfigurationExtensions
    {
        public static DataSource Environment(this AdvancedFiltersConfiguration c) => new EnvironmentDataSource
        (
            new AuthorizationAuthentication(c.Auth.CloudControlAuthScheme, c.Auth.CloudControlAuthParameter),
            new HostDataSourceRoute(c.Routes.Hosts.CloudControlHost)
        );
        public static DataSource AppInstance(this AdvancedFiltersConfiguration c) => new AppInstanceDataSource
        (
            new LuccaAuthentication(c.Auth.LuccaWebserviceToken),
            new HostDataSourceRoute(c.Routes.Tenants.AppInstancesEndpoint)
        );
        public static DataSource Establishment(this AdvancedFiltersConfiguration c) => new EstablishmentDataSource
        (
            new LuccaAuthentication(c.Auth.LuccaWebserviceToken),
            new HostDataSourceRoute(c.Routes.Tenants.EstablishmentsEndpoint)
        );
        public static DataSource LegalUnit(this AdvancedFiltersConfiguration c) => new LegalUnitDataSource
        (
            new LuccaAuthentication(c.Auth.LuccaWebserviceToken),
            new HostDataSourceRoute(c.Routes.Tenants.LegalUnitsEndpoint)
        );
    }
}
