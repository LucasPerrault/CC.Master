using System;
using System.Collections.Generic;

namespace AdvancedFilters.Domain.Billing.Models
{
    public class Client
    {
        public int RemoteId { get; set; }
        public Guid ExternalId { get; set; }
        public string Name { get; set; }

        public IReadOnlyCollection<Contract> Contracts { get; set; }
    }
}
