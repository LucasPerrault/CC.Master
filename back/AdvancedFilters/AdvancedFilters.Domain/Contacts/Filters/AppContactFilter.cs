using System.Collections.Generic;
using Tools;

namespace AdvancedFilters.Domain.Contacts.Filters
{
    public class AppContactFilter
    {
        public IReadOnlyCollection<string> ApplicationIds { get; set; }
        public IReadOnlyCollection<int> EnvironmentIds { get; set; }
        public CompareBoolean IsActive { get; set; } = CompareBoolean.Bypass;
        public CompareBoolean IsConfirmed { get; set; } = CompareBoolean.Bypass;
    }
}
