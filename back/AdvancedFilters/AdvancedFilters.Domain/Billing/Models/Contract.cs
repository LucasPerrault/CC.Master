using System;

namespace AdvancedFilters.Domain.Billing.Models
{
    public class Contract
    {
        public int Id { get; set; }
        public int RemoteId { get; set; }
        public Guid ExternalId { get; set; }
        public int ClientId { get; set; }

        public Client Client { get; set; }
    }
}
