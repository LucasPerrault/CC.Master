using System;
using System.Collections.Generic;

namespace AdvancedFilters.Domain.Billing.Models
{
    public class Contract
    {
        public int RemoteId { get; set; }
        public Guid ExternalId { get; set; }
        public int ClientId { get; set; }

        public Client Client { get; set; }
        public IReadOnlyCollection<EstablishmentContract> EstablishmentAttachments { get; set; }
    }
}
