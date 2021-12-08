using System.Collections.Generic;

namespace Billing.Contracts.Domain.Offers.Filtering
{
    public class CommercialOfferTagFilter
    {
        public HashSet<string> Search { get; set; } = new HashSet<string>();

        public static CommercialOfferTagFilter All => new CommercialOfferTagFilter();
    }
}
