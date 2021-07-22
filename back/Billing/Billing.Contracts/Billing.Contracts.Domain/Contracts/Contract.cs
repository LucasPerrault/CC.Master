using Billing.Contracts.Domain.Clients;
using Distributors.Domain.Models;
using System;
using System.Collections.Generic;

namespace Billing.Contracts.Domain.Contracts
{
    public class Contract
    {
        public int Id { get; set; }
        public Guid ExternalId { get; set; }
        public Guid ClientExternalId { get; set; }
        public int? EnvironmentId { get; set; }
        public string EnvironmentSubdomain { get; set; }
        public DateTime? ArchivedAt { get; set; }

        public int DistributorId { get; set; }
        public Distributor Distributor { get; set; }

        public int ClientId { get; set; }
        public Client Client { get; set; }

        public List<EstablishmentAttachment> Attachments { get; set; }
    }

    public class EstablishmentAttachment
    {
        public int Id { get; set; }
        public int ContractId { get; set; }
        public int EstablishmentId { get; set; }
        public int EstablishmentRemoteId { get; set; }
        public string EstablishmentName { get; set; }
        public DateTime StartsOn { get; set; }
        public DateTime? EndsOn { get; set; }
        public bool IsActive { get; set; }
    }
}
