using System.Collections.Generic;
using Tools;

namespace AdvancedFilters.Domain.Contacts.Filters
{
    public class SpecializedContactFilter
    {
        public IReadOnlyCollection<int> RoleIds { get; set; }
        public IReadOnlyCollection<int> EnvironmentIds { get; set; }
        public CompareBoolean IsActive { get; set; } = CompareBoolean.Bypass;
        public CompareBoolean IsConfirmed { get; set; } = CompareBoolean.Bypass;
    }
}
