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
    }

    public class HostRoutesConfiguration
    {
        public string CloudControlHost { get; set; }
    }
}
