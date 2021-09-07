using System;
using System.Collections.Generic;

namespace AdvancedFilters.Domain.Instance.Models
{
    public class Environment
    {
        public int Id { get; set; }
        public string Subdomain { get; set; }
        public string Domain { get; set; }
        public bool IsActive { get; set; }
        public string ProductionHost { get; set; }

        public IReadOnlyCollection<LegalUnit> LegalUnits { get; set; }
    }
}
