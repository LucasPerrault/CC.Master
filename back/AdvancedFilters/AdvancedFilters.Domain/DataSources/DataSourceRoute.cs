namespace AdvancedFilters.Domain.DataSources
{
    public enum DataSourceRouteType
    {
        Tenant,
        Host
    }

    public interface IDataSourceRoute
    {
        DataSourceRouteType Type { get; }
        string RequestUri { get; }
    }

    public class TenantDataSourceRoute : IDataSourceRoute
    {
        public DataSourceRouteType Type => DataSourceRouteType.Tenant;
        public string RequestUri => Endpoint;
        public string Endpoint { get; set; }
    }

    public class HostDataSourceRoute : IDataSourceRoute
    {

        public DataSourceRouteType Type => DataSourceRouteType.Host;
        public string RequestUri => Host;
        public string Host { get; }
        public HostDataSourceRoute(string host)
        {
            Host = host;
        }
    }
}
