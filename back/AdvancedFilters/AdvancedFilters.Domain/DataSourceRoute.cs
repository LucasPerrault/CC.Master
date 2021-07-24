namespace AdvancedFilters.Domain
{
    public enum DataSourceRouteType
    {
        Tenant,
        Host
    }

    public interface IDataSourceRoute
    {
        public DataSourceRouteType Type { get; }
    }

    public class TenantDataSourceRoute : IDataSourceRoute
    {
        public DataSourceRouteType Type => DataSourceRouteType.Tenant;
        public string Endpoint { get; set; }
    }

    public class HostDataSourceRoute : IDataSourceRoute
    {
        public DataSourceRouteType Type => DataSourceRouteType.Host;
        public string Host { get; set; }
    }
}
