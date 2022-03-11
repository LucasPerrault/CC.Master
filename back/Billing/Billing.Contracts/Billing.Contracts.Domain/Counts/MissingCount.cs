using Billing.Contracts.Domain.Common;
using Billing.Contracts.Domain.Contracts;

namespace Billing.Contracts.Domain.Counts;

public class MissingCount
{
    public AccountingPeriod Period { get; set; }
    public int ContractId { get; set; }

}
