using Tools;

namespace AdvancedFilters.Domain.Contacts.Filters
{
    public class SpecializedContactFilter
    {
        public int? RoleId { get; set; }
        public int? EnvironmentId { get; set; }
        public CompareBoolean IsActive { get; set; } = CompareBoolean.Bypass;
        public CompareBoolean IsConfirmed { get; set; } = CompareBoolean.Bypass;
    }
}
