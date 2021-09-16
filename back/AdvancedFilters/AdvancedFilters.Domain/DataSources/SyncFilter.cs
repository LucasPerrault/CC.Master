using System.Collections.Generic;

namespace AdvancedFilters.Domain.DataSources
{
    public class SyncFilter
    {
        public HashSet<string> Subdomains { get; set; } = new HashSet<string>();
    }
}
