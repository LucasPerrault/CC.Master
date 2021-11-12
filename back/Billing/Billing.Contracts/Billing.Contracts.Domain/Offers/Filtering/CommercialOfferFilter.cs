using System.Collections.Generic;

namespace Billing.Contracts.Domain.Offers.Filtering
{
    public class CommercialOfferFilter
    {
        public HashSet<string> Search { get; set; }
        public int? Id { get; set; }
        public HashSet<BillingMode> BillingModes { get; set; }
        public HashSet<string> Tags { get; set; }

        public static CommercialOfferFilter All => new CommercialOfferFilter();
    }
}
