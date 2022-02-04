using System.ComponentModel;
using Billing.Contracts.Domain.Common;
using Billing.Contracts.Domain.Contracts;

namespace Billing.Contracts.Domain.Counts
{
    public class Count
    {
        public int Id { get; set; }
        public int ContractId { get; set; }
        public AccountingPeriod CountPeriod { get; set; }
        public int CommercialOfferId { get; set; }
        public CountCode Code { get; set; }
        public Contract Contract { get; set; }
    }

    public enum CountCode
    {
        [Description("Count")]
        Count = 0,
        [Description("Draft")]
        Draft = 1
    }
}
