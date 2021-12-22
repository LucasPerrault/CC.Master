using System;

namespace Billing.Cmrr.Domain
{
    public enum BillingEntity
    {
        Unknown = 0,
        France = 1,
        Iberia = 2,
    }
    public class CmrrContract
    {
        public int Id { get; set; }
        public int ProductId { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public ContractCreationCause CreationCause { get; set; }
        public ContractEndReason EndReason { get; set; }

        public int ClientId { get; set; }
        public string ClientName { get; set; }
        public BillingEntity ClientBillingEntity { get; set; }
        public int DistributorId { get; set; }
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
