using System.Collections.Generic;

namespace Billing.Contracts.Domain.Offers.Filtering
{
    public class CommercialOfferFilter
    {
        public HashSet<string> Search { get; set; } = new HashSet<string>();
        public HashSet<int> Ids { get; set; }
        public HashSet<BillingMode> BillingModes { get; set; } = new HashSet<BillingMode>();
        public HashSet<string> Tags { get; set; } = new HashSet<string>();

        public static CommercialOfferFilter All => new CommercialOfferFilter();
    }
}
