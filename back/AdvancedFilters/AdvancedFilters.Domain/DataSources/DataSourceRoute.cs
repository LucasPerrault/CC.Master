using System;

namespace AdvancedFilters.Domain.DataSources
{

    public interface IDataSourceRoute
    {
        Uri GetUri<T, TContext>(TContext c) where TContext : IDataSourceContext<T>;
    }

    public class TenantDataSourceRoute : IDataSourceRoute
    {
        public string Endpoint { get; }

        public TenantDataSourceRoute(string endpoint)
        {
            Endpoint = endpoint;
        }

        public Uri GetUri<T, TContext>(TContext c) where TContext : IDataSourceContext<T>
        {
            return c.GetUri(this);
        }
    }

    public class HostDataSourceRoute : IDataSourceRoute
    {
        public Uri Uri { get; }

        public HostDataSourceRoute(Uri host, string endpoint)
        {
            Uri = new Uri(host, endpoint);
        }

        public Uri GetUri<T, TContext>(TContext c) where TContext : IDataSourceContext<T>
        {
            return c.GetUri(this);
        }
    }
}
