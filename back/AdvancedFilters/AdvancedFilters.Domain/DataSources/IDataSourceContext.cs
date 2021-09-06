using System;

namespace AdvancedFilters.Domain.DataSources
{
    public interface IDataSourceContext<in T>
    {
        public void Finalize(T item);
        public Uri GetUri(TenantDataSourceRoute route);
    }
}
