using System.Collections.Generic;
using Tools;

namespace Billing.Contracts.Domain.Offers.Filtering
{
    public class CommercialOfferFilter
    {
        public HashSet<string> Search { get; set; } = new HashSet<string>();
        public HashSet<int> Ids { get; set; } = new HashSet<int>();
        public HashSet<BillingMode> BillingModes { get; set; } = new HashSet<BillingMode>();
        public HashSet<PricingMethod> PriceMethods { get; set; } = new HashSet<PricingMethod>();
        public HashSet<ForecastMethod> ForecastMethods { get; set; } = new HashSet<ForecastMethod>();
        public HashSet<string> Tags { get; set; } = new HashSet<string>();
        public HashSet<int> ProductIds { get; set; } = new HashSet<int>();
        public HashSet<int> CurrencyIds { get; set; } = new HashSet<int>();
        public CompareBoolean IsArchived { get; set; } = CompareBoolean.Bypass;

        public static CommercialOfferFilter All => new CommercialOfferFilter();

        public static CommercialOfferFilter SimilarTo(CommercialOffer offer) => new CommercialOfferFilter
        {
            ProductIds = new HashSet<int> { offer.ProductId },
            BillingModes = new HashSet<BillingMode> { offer.BillingMode },
            PriceMethods = new HashSet<PricingMethod> { offer.PricingMethod },
            ForecastMethods = new HashSet<ForecastMethod> { offer.ForecastMethod },
            CurrencyIds = new HashSet<int> { offer.CurrencyId },
        };
        public static CommercialOfferFilter ForId(int id) => new CommercialOfferFilter { Ids = new HashSet<int> { id } };
    }
}
