using System;

namespace Billing.Cmrr.Domain
{
    public class CmrrContract
    {
        public int Id { get; set; }
        public int ProductId { get; set; }

        public DateTime CreationDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public ContractCreationCause CreationCause { get; set; }
        public ContractEndReason EndReason { get; set; }

        public int ClientId { get; set; }
        public string ClientName { get; set; }

        public int DistributorId { get; set; }

        public int EnvironmentId { get; set; }
        public DateTime EnvironmentCreatedAt { get; set; }

        public bool IsArchived { get; set; }

    }

    public enum ContractCreationCause : byte
    {
        NewBooking = 1,
        Modification = 2
    }

    public enum ContractEndReason
    {
        Modification = 1,
        Resiliation = 2
    };
}
