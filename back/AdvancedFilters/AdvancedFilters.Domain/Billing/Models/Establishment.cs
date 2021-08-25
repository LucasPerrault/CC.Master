using System;

namespace AdvancedFilters.Domain.Billing.Models
{
    public class Establishment
    {
        public int Id { get; set; }
        public int RemoteId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int LegalUnitId { get; set; }
        public string LegalIdentificationNumber { get; set; }
        public string ActivityCode { get; set; }
        public string TimeZoneId { get; set; }
        public int UsersCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsArchived { get; set; }

        public LegalUnit LegalUnit { get; set; }
    }
}
