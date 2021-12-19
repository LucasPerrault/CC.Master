using Billing.Contracts.Domain.Common;
using Billing.Contracts.Domain.Contracts;
using Billing.Contracts.Domain.Offers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Billing.Contracts.Domain.Counts.CreationStrategy
{
    public class CountContext
    {
        public Contract Contract { get; set; }
        public AccountingPeriod CountPeriod { get; set; }
        public List<int> OtherCountNumberFromContractGroup { get; set; }
        public PriceList PriceList { get; set; }
    }

    public abstract class CountCreationStrategy
    {
        public abstract Task<Count> MakeCountAsync(AccountingPeriod countPeriod, ContractWithCountNumber contractWithCountNumber, List<ContractWithCountNumber> otherFromContractGroup);
    }

    public static class CountsExtensions
    {
        public static bool HasPriorityOver(this Count count, Count otherCount)
        {
            return count.TotalInCurrency > otherCount.TotalInCurrency
                   || ( count.TotalInCurrency == otherCount.TotalInCurrency && !count.IsMinimalBilling );
        }

        public static Count GetHighestPriority(this IEnumerable<Count> counts)
        {
            if (!counts.Any())
            {
                throw new ApplicationException("At least one count is necessary for comparison");
            }

            return counts.Aggregate((count, otherCount) => count.HasPriorityOver(otherCount) ? count : otherCount);
        }
    }
}
