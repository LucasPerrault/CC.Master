using System.Collections.Generic;
using Billing.Contracts.Domain.Common;

namespace Billing.Contracts.Domain.Counts.Filtering
{
    public class CountFilter
    {
        public HashSet<int> Ids { get; set; } = new HashSet<int>();
        public HashSet<int> CommercialOfferIds { get; set; } = new HashSet<int>();
        public HashSet<int> ContractIds { get; set; } = new HashSet<int>();
        public HashSet<CountCode> Codes { get; set; } = new HashSet<CountCode>();
        public HashSet<AccountingPeriod> Periods { get; set; } = new HashSet<AccountingPeriod>();
        public static CountFilter All => new CountFilter();
    }
}
