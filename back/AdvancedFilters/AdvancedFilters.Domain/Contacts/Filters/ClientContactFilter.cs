using Tools;

namespace AdvancedFilters.Domain.Contacts.Filters
{
    public class ClientContactFilter
    {
        public int? RoleId { get; set; }
        public CompareString ClientId { get; set; } = CompareString.Bypass;
        public int? EnvironmentId { get; set; }
        public int? EstablishmentId { get; set; }
        public CompareBoolean IsActive { get; set; } = CompareBoolean.Bypass;
        public CompareBoolean IsConfirmed { get; set; } = CompareBoolean.Bypass;
    }
}
