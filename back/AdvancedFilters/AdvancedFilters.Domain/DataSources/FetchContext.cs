using System;

namespace AdvancedFilters.Domain.DataSources
{
    public interface IFetchContextPayload<in T>
    {
        public void Finalize(T item);
        public Uri GetUri(TenantDataSourceRoute route);
    }
}
