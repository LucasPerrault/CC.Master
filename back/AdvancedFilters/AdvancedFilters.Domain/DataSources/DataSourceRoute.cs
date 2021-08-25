namespace AdvancedFilters.Domain.DataSources
{
    public interface IDataSourceRoute
    { }

    public class TenantDataSourceRoute : IDataSourceRoute
    {
        public string Endpoint { get; }

        public TenantDataSourceRoute(string endpoint)
        {
            Endpoint = endpoint;
        }
    }

    public class HostDataSourceRoute : IDataSourceRoute
    {
        public string Host { get; }

        public HostDataSourceRoute(string host)
        {
            Host = host;
        }
    }
}
