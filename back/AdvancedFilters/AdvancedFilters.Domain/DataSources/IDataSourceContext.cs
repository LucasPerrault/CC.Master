using System;

namespace AdvancedFilters.Domain.DataSources
{
    public interface IDataSourceContext<in T>
    {
        void Finalize(T item);
        Uri GetUri(TenantDataSourceRoute route);
        Uri GetUri(HostDataSourceRoute hostDataSourceRoute);
    }
}
