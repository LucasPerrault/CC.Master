using System;
using System.Collections.Generic;
using Tools;

namespace Billing.Contracts.Domain.Offers.Comparisons
{
    public class PriceListComparisonObject : ValueObject
    {
        public DateTime StartsOn { get; }

        protected override IEnumerable<object> EqualityComponents
        {
            get
            {
                yield return StartsOn;
            }
        }

        public PriceListComparisonObject(PriceList priceList)
        {
            StartsOn = priceList.StartsOn;
        }
    }
}
