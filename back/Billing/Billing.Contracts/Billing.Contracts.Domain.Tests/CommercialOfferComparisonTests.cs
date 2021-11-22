using Billing.Contracts.Domain.Offers;
using Billing.Contracts.Domain.Offers.Comparisons;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Billing.Contracts.Domain.Tests
{
    public class CommercialOfferComparisonTests
    {
        [Fact]
        public void ShouldComparison_ApplyTo_OfferProperties()
        {
            var offerProps = typeof(CommercialOffer).GetProperties().Select(info => info.Name);
            var comparisonProps = typeof(CommercialOfferComparisonObject).GetProperties().Select(info => info.Name);
            var comparisonExcludedProps = new List<string>
            {
                nameof(CommercialOffer.Id),
                nameof(CommercialOffer.Name),
                nameof(CommercialOffer.IsCatalog),
                nameof(CommercialOffer.PriceLists),
                nameof(CommercialOffer.Product),
            };

            var diff = offerProps
                .Except(comparisonProps)
                .Except(comparisonExcludedProps);

            diff.Should().BeEmpty();
        }

        [Fact]
        public void ShouldComparison_ApplyTo_PriceListProperties()
        {
            var priceListProps = typeof(PriceList).GetProperties().Select(info => info.Name);
            var comparisonProps = typeof(PriceListComparisonObject).GetProperties().Select(info => info.Name);
            var comparisonExcludedProps = new List<string>
            {
                nameof(PriceList.Id),
                nameof(PriceList.OfferId),
                nameof(PriceList.Rows),
            };

            var diff = priceListProps
                .Except(comparisonProps)
                .Except(comparisonExcludedProps);

            diff.Should().BeEmpty();
        }

        [Fact]
        public void ShouldComparison_ApplyTo_PriceRowProperties()
        {
            var priceRowProps = typeof(PriceRow).GetProperties().Select(info => info.Name);
            var comparisonProps = typeof(PriceRowComparisonObject).GetProperties().Select(info => info.Name);
            var comparisonExcludedProps = new List<string>
            {
                nameof(PriceRow.Id),
                nameof(PriceRow.ListId),
            };

            var diff = priceRowProps
                .Except(comparisonProps)
                .Except(comparisonExcludedProps);

            diff.Should().BeEmpty();
        }
    }
}
