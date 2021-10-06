using System.Collections.Generic;

namespace AdvancedFilters.Domain.DataSources
{
    public enum DataSourceSyncMode
    {
        Everything,
        MonoTenant,
        MultiTenant,
    }
    public class SyncFilter
    {
        public HashSet<string> Subdomains { get; set; } = new HashSet<string>();
        public DataSourceSyncMode SyncMode { get; set; } = DataSourceSyncMode.Everything;
    }
}
