using System.Collections.Generic;

namespace AdvancedFilters.Domain.Instance.Filters
{
    public class AppInstanceFilter
    {
        public IReadOnlyCollection<int> RemoteIds { get; set; }
        public IReadOnlyCollection<string> ApplicationIds { get; set; }
        public IReadOnlyCollection<int> EnvironmentIds { get; set; }
    }
}
