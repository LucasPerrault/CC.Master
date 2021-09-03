using System;
using System.Collections.Generic;

namespace AdvancedFilters.Domain.Instance.Models
{
    public class LegalUnit
    {
        public int RemoteId { get; set; }
        public int EnvironmentId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string LegalIdentificationNumber { get; set; }
        public string ActivityCode { get; set; }
        public int CountryId { get; set; }
        public int HeadquartersId { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsArchived { get; set; }

        public Environment Environment { get; set; }
        public IReadOnlyCollection<Establishment> Establishments { get; set; }
    }
}
