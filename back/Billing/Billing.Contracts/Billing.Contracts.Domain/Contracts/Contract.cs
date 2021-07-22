using Distributors.Domain.Models;
using System;

namespace Billing.Contracts.Domain.Contracts
{
    public class Contract
    {
        public int Id { get; set; }
        public Guid ExternalId { get; set; }
        public int ClientId { get; set; }
        public Guid ClientExternalId { get; set; }
        public int? EnvironmentId { get; set; }
        public string EnvironmentSubdomain { get; set; }
        public DateTime? ArchivedAt { get; set; }

        public int DistributorId { get; set; }
        public Distributor Distributor { get; set; }
    }
}
