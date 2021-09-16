using System.Collections.Generic;
using Tools;

namespace AdvancedFilters.Domain.Instance.Filters
{
    public class EnvironmentFilter
    {
        public string Search { get; set; }
        public HashSet<string> Subdomains { get; set; } = new HashSet<string>();
        public HashSet<string> Domains { get; set; } = new HashSet<string>();
        public CompareBoolean IsActive { get; set; } = CompareBoolean.Bypass;
    }
}
