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
        public string Tag { get; }
        public int CurrencyId { get; }
        public bool IsArchived { get; }

        protected override IEnumerable<object> EqualityComponents
        {
            get
            {
                yield return ProductId;
                yield return BillingMode;
                yield return Unit;
                yield return PricingMethod;
                yield return ForecastMethod;
                yield return Tag;
                yield return CurrencyId;
                yield return IsArchived;
            }
        }

        public CommercialOfferComparisonObject(CommercialOffer offer)
        {
            ProductId = offer.ProductId;
            BillingMode = offer.BillingMode;
            Unit = offer.Unit;
            PricingMethod = offer.PricingMethod;
            ForecastMethod = offer.ForecastMethod;
            Tag = offer.Tag;
            CurrencyId = offer.CurrencyId;
            IsArchived = offer.IsArchived;
        }
    }
}
