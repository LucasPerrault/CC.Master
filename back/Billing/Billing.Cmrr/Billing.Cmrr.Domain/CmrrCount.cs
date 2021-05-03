using System;

namespace Billing.Cmrr.Domain
{
    public class CmrrCount
    {
        public int Id { get; set; }

        public int ContractId { get; set; }
        public DateTime CountPeriod { get; set; }
        public BillingStrategy BillingStrategy { get; set; }

        public string AccountingNumber { get; set; }
        public int EntryNumber { get; set; }
        public string Code { get; set; }

        public int CurrencyId { get; set; }
        public decimal CurrencyTotal { get; set; }
        public decimal EuroTotal { get; set; }
        public decimal LuccaDiscount { get; set; }
        public decimal DistributorDiscount { get; set; }

        public DateTime CreatedAt { get; set; }
    }

    public enum BillingStrategy
    {
        Standard = 0,
        MinimalBilling = 1
    }
}
