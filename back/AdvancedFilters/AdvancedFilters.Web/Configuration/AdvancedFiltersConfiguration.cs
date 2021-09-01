using AdvancedFilters.Domain.Billing;
using AdvancedFilters.Domain.Contacts;
using AdvancedFilters.Domain.DataSources;
using AdvancedFilters.Domain.Instance;
using System;

namespace AdvancedFilters.Web.Configuration
{
    public class AdvancedFiltersConfiguration
    {
        public RoutesConfiguration Routes { get; set; }
        public AuthenticationConfiguration Auth { get; set; }
    }

    internal static class AdvancedFiltersConfigurationExtensions
    {
        public static DataSource Environment(this AdvancedFiltersConfiguration c) => new EnvironmentDataSource
        (
            new AuthorizationAuthentication(c.Auth.CloudControlAuthScheme, c.Auth.CloudControlAuthParameter),
            new HostDataSourceRoute(c.Routes.Hosts.CloudControl.Host, c.Routes.Hosts.CloudControl.EnvironmentsEndpoint)
        );

        public static DataSource AppInstance(this AdvancedFiltersConfiguration c) => new AppInstanceDataSource
        (
            new LuccaAuthentication(c.Auth.LuccaWebserviceToken),
            new TenantDataSourceRoute(c.Routes.Tenants.AppInstancesEndpoint)
        );

        public static DataSource Establishment(this AdvancedFiltersConfiguration c) => new EstablishmentDataSource
        (
            new LuccaAuthentication(c.Auth.LuccaWebserviceToken),
            new TenantDataSourceRoute(c.Routes.Tenants.EstablishmentsEndpoint)
        );

        public static DataSource LegalUnit(this AdvancedFiltersConfiguration c) => new LegalUnitDataSource
        (
            new LuccaAuthentication(c.Auth.LuccaWebserviceToken),
            new TenantDataSourceRoute(c.Routes.Tenants.LegalUnitsEndpoint)
        );

        public static DataSource Contract(this AdvancedFiltersConfiguration c) => new ContractDataSource
        (
            new AuthorizationAuthentication(c.Auth.CloudControlAuthScheme, c.Auth.CloudControlAuthParameter),
            new HostDataSourceRoute(c.Routes.Hosts.CloudControl.Host, c.Routes.Hosts.CloudControl.ContractsEndpoint)
        );

        public static DataSource Client(this AdvancedFiltersConfiguration c) => new ClientDataSource
        (
            new AuthorizationAuthentication(c.Auth.CloudControlAuthScheme, c.Auth.CloudControlAuthParameter),
            new HostDataSourceRoute(c.Routes.Hosts.CloudControl.Host, c.Routes.Hosts.CloudControl.ClientsEndpoint)
        );

        public static DataSource AppContact(this AdvancedFiltersConfiguration c) => new AppContactDataSource
        (
            new LuccaAuthentication(c.Auth.LuccaWebserviceToken),
            new TenantDataSourceRoute(c.Routes.Tenants.AppContactsEndpoint)
        );

        public static DataSource ClientContact(this AdvancedFiltersConfiguration c) => new ClientContactDataSource
        (
            new LuccaAuthentication(c.Auth.LuccaWebserviceToken),
            new TenantDataSourceRoute(c.Routes.Tenants.ClientContactsEndpoint)
        );

        public static DataSource SpecializedContact(this AdvancedFiltersConfiguration c) => new SpecializedContactDataSource
        (
            new LuccaAuthentication(c.Auth.LuccaWebserviceToken),
            new TenantDataSourceRoute(c.Routes.Tenants.SpecializedContactsEndpoint)
        );
    }
}
