using System.Collections.Generic;
using Tools;

namespace Billing.Contracts.Domain.Counts.Filtering
{
    public class CountFilter
    {
        public HashSet<int> Ids { get; set; } = new HashSet<int>();
        public HashSet<int> CommercialOfferIds { get; set; } = new HashSet<int>();
        public HashSet<int> ContractIds { get; set; } = new HashSet<int>();
        public CompareDateTime CountPeriod { get; set; }
        public static CountFilter All => new CountFilter();
    }
}
