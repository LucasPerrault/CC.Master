using Billing.Contracts.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Billing.Contracts.Domain.Environments
{
    public class ContractEnvironment
    {
        public int Id { get; set; }
        public string Subdomain { get; set; }
        public List<Establishment> Establishments { get; set; }
    }

    public class Establishment
    {
        public int Id { get; set; }
        public int EnvironmentId { get; set; }
        public int RemoteId { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public int? LegalUnitId { get; set; }

        public List<EstablishmentExclusion> Exclusions { get; set; }
        public List<EstablishmentAttachment> Attachments { get; set; }
    }

    public class EstablishmentExclusion
    {
        public int Id { get; set; }
        public int EstablishmentId { get; set; }
        public int ProductId { get; set; }
        public int AuthorId { get; set; }
        public DateTime CreatedAt { get; set; }
    }


    public class EstablishmentAttachment
    {
        public int Id { get; set; }
        public int EstablishmentId { get; set; }
        public int EstablishmentRemoteId { get; set; }
        public string EstablishmentName { get; set; }
        public DateTime StartsOn { get; set; }
        public DateTime? EndsOn { get; set; }

        public int ContractId { get; set; }
        [JsonIgnore] public Contract Contract { get; set; }
        public int? ProductId => Contract?.CommercialOffer?.ProductId;

        public string EndReason { get; set; }
        public int NumberOfFreeMonths { get; set; }
    }
}
