using System;
using System.Collections.Generic;
using Environment = AdvancedFilters.Domain.Instance.Models.Environment;

namespace AdvancedFilters.Domain.Billing.Models
{
    public class Contract
    {
        public int Id { get; set; }
        public Guid ExternalId { get; set; }
        public int ClientId { get; set; }
        public int EnvironmentId { get; set; }

        public Client Client { get; set; }
        public IEnumerable<EstablishmentContract> EstablishmentAttachments { get; set; }

        public Environment Environment { get; set; }
    }
}
