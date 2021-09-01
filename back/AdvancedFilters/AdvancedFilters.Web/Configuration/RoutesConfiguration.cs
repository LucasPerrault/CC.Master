using System;

namespace AdvancedFilters.Web.Configuration
{
    public class RoutesConfiguration
    {

        public HostRoutesConfiguration Hosts { get; set; }
        public TenantRoutesConfiguration Tenants { get; set; }
    }

    public class TenantRoutesConfiguration
    {
        public string EstablishmentsEndpoint { get; set; }
        public string AppInstancesEndpoint { get; set; }
        public string LegalUnitsEndpoint { get; set; }
        public string AppContactsEndpoint { get; set; }
        public string ClientContactsEndpoint { get; set; }
        public string SpecializedContactsEndpoint { get; set; }
    }

    public class HostRoutesConfiguration
    {
        public CloudControlHostConfiguration CloudControl { get; set; }
    }

    public class CloudControlHostConfiguration
    {
        public Uri Host { get; set; }
        public string EnvironmentsEndpoint { get; set; }
        public string ContractsEndpoint { get; set; }
        public string ClientsEndpoint { get; set; }
    }
}
