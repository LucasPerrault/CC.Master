using System.Collections.Generic;

namespace Billing.Contracts.Domain.Offers.Filtering
{
    public class CommercialOfferUsageFilter
    {
        public HashSet<int> OfferIds { get; set; } = new HashSet<int>();

        public static CommercialOfferUsageFilter All => new CommercialOfferUsageFilter();
    }
}
