using System;

namespace AdvancedFilters.Domain.DataSources
{

    public interface IDataSourceRoute
    {
        Uri GetUri<T, TPayload>(TPayload p) where TPayload : IFetchContextPayload<T>;
    }

    public class TenantDataSourceRoute : IDataSourceRoute
    {
        public string Endpoint { get; }

        public TenantDataSourceRoute(string endpoint)
        {
            Endpoint = endpoint;
        }

        public Uri GetUri<T, TPayload>(TPayload p) where TPayload : IFetchContextPayload<T>
        {
            return p.GetUri(this);
        }
    }

    public class HostDataSourceRoute : IDataSourceRoute
    {
        public Uri Uri { get; }

        public HostDataSourceRoute(Uri host, string endpoint)
        {
            Uri = new Uri(host, endpoint);
        }

        public Uri GetUri<T, TPayload>(TPayload p) where TPayload : IFetchContextPayload<T>
        {
            return Uri;
        }
    }
}
