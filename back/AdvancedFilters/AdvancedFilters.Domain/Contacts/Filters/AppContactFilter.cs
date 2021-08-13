using Tools;

namespace AdvancedFilters.Domain.Contacts.Filters
{
    public class AppContactFilter
    {
        public CompareString ApplicationId { get; set; } = CompareString.Bypass;
        public int? EnvironmentId { get; set; }
        public CompareBoolean IsActive { get; set; } = CompareBoolean.Bypass;
        public CompareBoolean IsConfirmed { get; set; } = CompareBoolean.Bypass;
    }
}
