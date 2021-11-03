using AdvancedFilters.Domain.Billing;
using AdvancedFilters.Domain.Contacts;
using AdvancedFilters.Domain.Core;
using AdvancedFilters.Domain.DataSources;
using AdvancedFilters.Domain.Instance;

namespace AdvancedFilters.Web.Configuration
{
    public class AdvancedFiltersConfiguration
    {
        public int MaxParallelHttpCalls { get; set; }
        public RoutesConfiguration Routes { get; set; }
        public AuthenticationConfiguration Auth { get; set; }
    }

    internal static class AdvancedFiltersConfigurationExtensions
    {
        public static RemoteDataSource Environment(this AdvancedFiltersConfiguration c) => new EnvironmentDataSource
        (
            new AuthorizationAuthentication(c.Auth.CloudControlAuthScheme, c.Auth.CloudControlAuthParameter),
            new HostDataSourceRoute(c.Routes.Hosts.CloudControl.Host, c.Routes.Hosts.CloudControl.EnvironmentsEndpoint)
        );

        public static LocalDataSource Country(this AdvancedFiltersConfiguration c) => new CountryDataSource();

        public static RemoteDataSource AppInstance(this AdvancedFiltersConfiguration c) => new AppInstanceDataSource
        (
            new LuccaAuthentication(c.Auth.MonolithWebserviceToken),
            new TenantDataSourceRoute(c.Routes.Tenants.AppInstancesEndpoint)
        );

        public static RemoteDataSource Establishment(this AdvancedFiltersConfiguration c) => new EstablishmentDataSource
        (
            new LuccaAuthentication(c.Auth.OrganizationStructureWebserviceToken),
            new TenantDataSourceRoute(c.Routes.Tenants.EstablishmentsEndpoint)
        );

        public static RemoteDataSource LegalUnit(this AdvancedFiltersConfiguration c) => new LegalUnitDataSource
        (
            new LuccaAuthentication(c.Auth.OrganizationStructureWebserviceToken),
            new TenantDataSourceRoute(c.Routes.Tenants.LegalUnitsEndpoint)
        );

        public static RemoteDataSource Contract(this AdvancedFiltersConfiguration c) => new ContractDataSource
        (
            new AuthorizationAuthentication(c.Auth.CloudControlAuthScheme, c.Auth.CloudControlAuthParameter),
            new HostDataSourceRoute(c.Routes.Hosts.CloudControl.Host, c.Routes.Hosts.CloudControl.ContractsEndpoint)
        )
        {
            SubdomainsParamName = c.Routes.Hosts.CloudControl.ContractsSubdomainParamName
        };

        public static RemoteDataSource Client(this AdvancedFiltersConfiguration c) => new ClientDataSource
        (
            new AuthorizationAuthentication(c.Auth.CloudControlAuthScheme, c.Auth.CloudControlAuthParameter),
            new HostDataSourceRoute(c.Routes.Hosts.CloudControl.Host, c.Routes.Hosts.CloudControl.ClientsEndpoint)
        );

        public static RemoteDataSource Distributor(this AdvancedFiltersConfiguration c) => new DistributorDataSource
        (
            new AuthorizationAuthentication(c.Auth.CloudControlAuthScheme, c.Auth.CloudControlAuthParameter),
            new HostDataSourceRoute(c.Routes.Hosts.CloudControl.Host, c.Routes.Hosts.CloudControl.DistributorsEndpoint)
        );

        public static RemoteDataSource EnvironmentAccess(this AdvancedFiltersConfiguration c) => new EnvironmentAccessDataSource
        (
            new AuthorizationAuthentication(c.Auth.CloudControlAuthScheme, c.Auth.CloudControlAuthParameter),
            new HostDataSourceRoute(c.Routes.Hosts.CloudControl.Host, c.Routes.Hosts.CloudControl.EnvironmentAccessesEndpoint)
        );

        public static RemoteDataSource AppContact(this AdvancedFiltersConfiguration c) => new AppContactDataSource
        (
            new LuccaAuthentication(c.Auth.ClientCenterWebserviceToken),
            new TenantDataSourceRoute(c.Routes.Tenants.AppContactsEndpoint)
        );

        public static RemoteDataSource ClientContact(this AdvancedFiltersConfiguration c) => new ClientContactDataSource
        (
            new LuccaAuthentication(c.Auth.ClientCenterWebserviceToken),
            new TenantDataSourceRoute(c.Routes.Tenants.ClientContactsEndpoint)
        );

        public static RemoteDataSource SpecializedContact(this AdvancedFiltersConfiguration c) => new SpecializedContactDataSource
        (
            new LuccaAuthentication(c.Auth.ClientCenterWebserviceToken),
            new TenantDataSourceRoute(c.Routes.Tenants.SpecializedContactsEndpoint)
        );
    }
}
