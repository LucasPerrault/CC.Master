using Billing.Contracts.Domain.Offers.Parsing;
using Billing.Contracts.Domain.Offers.Parsing.Exceptions;
using Billing.Products.Domain;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Billing.Contracts.Domain.Tests.Offers.Parsing
{
    public class ParsedOffersServiceTests
    {
        [Fact]
        public void ShouldConvertToParsedOffersWithOneOfferContainingTwoPriceLists()
        {
            var offerRows = new List<OfferRow>
            {
                new OfferRow
                {
                    Name = "newOffer",
                    ProductName = "cleemy",
                    Category = "category",
                    Currency = ParsedCurrency.EUR,
                    BillingMode = ParsedBillingMode.AllUsers,
                    BillingUnit = ParsedBillingUnit.Cards,
                    ForecastMethod = ParsedForecastMethod.AnnualCommitment,
                    PricingMethod = ParsedPricingMethod.AnnualCommitment,
                    StartsOn = new DateTime(2021,01,01),
                    MinIncludedCount = 0,
                    MaxIncludedCount = 10,
                    FixedPrice = 15,
                    UnitPrice = 5
                },
                new OfferRow
                {
                    MinIncludedCount = 11,
                    MaxIncludedCount = 20,
                    UnitPrice = 3,
                    FixedPrice = 14
                },
                new OfferRow
                {
                    MinIncludedCount = 21,
                    MaxIncludedCount = 300,
                    UnitPrice = 2,
                    FixedPrice = 10
                },
                new OfferRow
                {
                    StartsOn = new DateTime(2021, 02,01),
                    MinIncludedCount = 0,
                    MaxIncludedCount = 20,
                    UnitPrice = 2,
                    FixedPrice = 12
                },
                new OfferRow
                {
                    MinIncludedCount = 21,
                    MaxIncludedCount = 300,
                    UnitPrice = 1,
                    FixedPrice = 11
                }
            };

            var products = new List<Product>
            {
                new Product
                {
                    Id = 1,
                    Name = "cleemy"
                }
            };

            var sut = new ParsedOffersService();

            var parsedOffers = sut.ConvertToParsedOffers(offerRows, products);

            parsedOffers.Should().HaveCount(1);

            var parsedOffer = parsedOffers.First();

            parsedOffer.PriceLists.Should().HaveCount(2);

            parsedOffer.PriceLists.Should().ContainSingle(x => x.Rows.Count == 3);
            parsedOffer.PriceLists.Should().ContainSingle(x => x.Rows.Count == 2);
        }

        [Fact]
        public void ShouldConvertToParsedOffersWithTwoOffers()
        {
            var offerRows = new List<OfferRow>
            {
                new OfferRow
                {
                    Name = "newOffer",
                    ProductName = "cleemy",
                    Category = "category",
                    Currency = ParsedCurrency.EUR,
                    BillingMode = ParsedBillingMode.AllUsers,
                    BillingUnit = ParsedBillingUnit.Cards,
                    ForecastMethod = ParsedForecastMethod.AnnualCommitment,
                    PricingMethod = ParsedPricingMethod.AnnualCommitment,
                    StartsOn = new DateTime(2021,01,01),
                    MinIncludedCount = 0,
                    MaxIncludedCount = 10,
                    FixedPrice = 15,
                    UnitPrice = 5
                },
                new OfferRow
                {
                    MinIncludedCount = 11,
                    MaxIncludedCount = 20,
                    UnitPrice = 3,
                    FixedPrice = 14
                },
                new OfferRow
                {
                    Name = "newOffer2",
                    ProductName = "cleemy",
                    Category = "category2",
                    Currency = ParsedCurrency.EUR,
                    BillingMode = ParsedBillingMode.FlatFee,
                    BillingUnit = ParsedBillingUnit.Declarers,
                    ForecastMethod = ParsedForecastMethod.LastRealMonth,
                    PricingMethod = ParsedPricingMethod.Linear,
                    StartsOn = new DateTime(2021,01,01),
                    MinIncludedCount = 0,
                    MaxIncludedCount = 10,
                    FixedPrice = 15,
                    UnitPrice = 5
                },
                new OfferRow
                {
                    MinIncludedCount = 11,
                    MaxIncludedCount = 20,
                    UnitPrice = 3,
                    FixedPrice = 14
                },
            };

            var products = new List<Product>
            {
                new Product
                {
                    Id = 1,
                    Name = "cleemy"
                }
            };

            var sut = new ParsedOffersService();

            var parsedOffers = sut.ConvertToParsedOffers(offerRows, products);

            parsedOffers.Should().HaveCount(2);
        }

        [Fact]
        public void ShouldThrowWhenConvertToParsedOffersWithBadPriceLists()
        {
            var offerRows = new List<OfferRow>
            {
                new OfferRow
                {
                    Name = "newOffer",
                    ProductName = "cleemy",
                    Category = "category",
                    Currency = ParsedCurrency.EUR,
                    BillingMode = ParsedBillingMode.AllUsers,
                    BillingUnit = ParsedBillingUnit.Cards,
                    ForecastMethod = ParsedForecastMethod.AnnualCommitment,
                    PricingMethod = ParsedPricingMethod.AnnualCommitment,
                    StartsOn = new DateTime(2021,01,01),
                    MinIncludedCount = 0,
                    MaxIncludedCount = 10,
                    FixedPrice = 15,
                    UnitPrice = 5
                },
                new OfferRow
                {
                    MinIncludedCount = 12,
                    MaxIncludedCount = 20,
                    UnitPrice = 3,
                    FixedPrice = 14
                },
            };

            var products = new List<Product>
            {
                new Product
                {
                    Id = 1,
                    Name = "cleemy"
                }
            };

            var sut = new ParsedOffersService();

            Func<List<ParsedOffer>> func = () => sut.ConvertToParsedOffers(offerRows, products);

            func.Should().ThrowExactly<PriceRowsCoherencyException>();
        }
    }
}
