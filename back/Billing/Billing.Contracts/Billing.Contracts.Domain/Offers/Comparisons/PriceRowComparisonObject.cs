using System.Collections.Generic;
using Tools;

namespace Billing.Contracts.Domain.Offers.Comparisons
{
    public class PriceRowComparisonObject : ValueObject
    {
        public int MaxIncludedCount { get; }
        public decimal UnitPrice { get; }
        public decimal FixedPrice { get; }

        protected override IEnumerable<object> EqualityComponents
        {
            get
            {
                yield return MaxIncludedCount;
                yield return UnitPrice;
                yield return FixedPrice;
            }
        }

        public PriceRowComparisonObject(PriceRow row)
        {
            MaxIncludedCount = row.MaxIncludedCount;
            UnitPrice = row.UnitPrice;
            FixedPrice = row.FixedPrice;
        }
    }
}
