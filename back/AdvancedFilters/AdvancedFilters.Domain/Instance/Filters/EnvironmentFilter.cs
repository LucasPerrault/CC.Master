using Tools;

namespace AdvancedFilters.Domain.Instance.Filters
{
    public class EnvironmentFilter
    {
        public string Search { get; set; }
        public CompareString Subdomain { get; set; } = CompareString.Bypass;
        public CompareString Domain { get; set; } = CompareString.Bypass;
        public CompareBoolean IsActive { get; set; } = CompareBoolean.Bypass;
    }
}
