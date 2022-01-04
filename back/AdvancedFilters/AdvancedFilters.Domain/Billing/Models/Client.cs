using System;
using System.Collections.Generic;

namespace AdvancedFilters.Domain.Billing.Models
{
    public class Client
    {
        public int Id { get; set; }
        public Guid ExternalId { get; set; }
        public string Name { get; set; }

        public IEnumerable<Contract> Contracts { get; set; }
    }
}
