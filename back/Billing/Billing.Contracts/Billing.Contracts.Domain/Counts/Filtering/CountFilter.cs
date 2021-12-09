using System.Collections.Generic;

namespace Billing.Contracts.Domain.Counts.Filtering
{
    public class CountFilter
    {
        public HashSet<int> Ids { get; set; } = new HashSet<int>();
        public HashSet<int> CommercialOfferIds { get; set; } = new HashSet<int>();
        public HashSet<int> ContractIds { get; set; } = new HashSet<int>();

        public static CountFilter All => new CountFilter();
    }
}
