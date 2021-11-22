using System.Collections.Generic;

namespace Billing.Contracts.Domain.Offers.Filtering
{
    public class CommercialOfferFilter
    {
        public HashSet<string> Search { get; set; } = new HashSet<string>();
        public HashSet<int> Ids { get; set; } = new HashSet<int>();
        public HashSet<BillingMode> BillingModes { get; set; } = new HashSet<BillingMode>();
        public HashSet<string> Tags { get; set; } = new HashSet<string>();
        public HashSet<int> ProductIds { get; set; } = new HashSet<int>();
        public HashSet<int> CurrencyIds { get; set; } = new HashSet<int>();

        public static CommercialOfferFilter All => new CommercialOfferFilter();
    }
}
