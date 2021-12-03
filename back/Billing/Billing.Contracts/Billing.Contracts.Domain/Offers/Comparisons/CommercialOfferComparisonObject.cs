using System.Collections.Generic;
using Tools;

namespace Billing.Contracts.Domain.Offers.Comparisons
{
    public class CommercialOfferComparisonObject : ValueObject
    {
        public int ProductId { get; }
        public BillingMode BillingMode { get; }
        public BillingUnit Unit { get; }
        public PricingMethod PricingMethod { get; }
        public ForecastMethod ForecastMethod { get; }
        public int CurrencyId { get; }

        protected override IEnumerable<object> EqualityComponents
        {
            get
            {
                yield return ProductId;
                yield return BillingMode;
                yield return Unit;
                yield return PricingMethod;
                yield return ForecastMethod;
                yield return CurrencyId;
            }
        }

        public CommercialOfferComparisonObject(CommercialOffer offer)
        {
            ProductId = offer.ProductId;
            BillingMode = offer.BillingMode;
            Unit = offer.Unit;
            PricingMethod = offer.PricingMethod;
            ForecastMethod = offer.ForecastMethod;
            CurrencyId = offer.CurrencyId;
        }
    }
}
