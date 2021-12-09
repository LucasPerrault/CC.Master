using Billing.Contracts.Domain.Common;

namespace Billing.Contracts.Domain.Counts
{
    public class Count
    {
        public int Id { get; set; }
        public int ContractId { get; set; }
        public AccountingPeriod CountPeriod { get; set; }
        public int CommercialOfferId { get; set; }
    }
}
