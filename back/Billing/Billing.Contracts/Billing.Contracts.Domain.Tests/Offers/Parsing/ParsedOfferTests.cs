using Billing.Contracts.Domain.Offers;
using Billing.Contracts.Domain.Offers.Parsing;
using Billing.Products.Domain;
using FluentAssertions;
using Xunit;

namespace Billing.Contracts.Domain.Tests.Offers.Parsing
{
    public class ParsedOfferTests
    {
        [Fact]
        public void ShouldCreateParsedOffer()
        {
            var row = new OfferRow
            {
                PricingMethod = ParsedPricingMethod.AnnualCommitment,
                BillingMode = ParsedBillingMode.ActiveUsers,
                BillingUnit = ParsedBillingUnit.ActiveUsers,
                ForecastMethod = ParsedForecastMethod.AnnualCommitment,
                Category = "category",
                Currency = ParsedCurrency.EUR,
                Name = "offer",
                ProductName = "cleemy",
            };

            var product = new Product { Id = 1 };

            var parsedOffer = new ParsedOffer(row, product);

            parsedOffer.Should().NotBeNull();
            parsedOffer.Category.Should().Be(row.Category);
            parsedOffer.CurrencyId.Should().Be((int)row.Currency);
            parsedOffer.Name.Should().Be(row.Name);
            parsedOffer.Product.Should().Be(product);
        }


        [Theory]
        [InlineData(ParsedPricingMethod.AnnualCommitment, PricingMethod.AnnualCommitment)]
        [InlineData(ParsedPricingMethod.Constant, PricingMethod.Constant)]
        [InlineData(ParsedPricingMethod.Linear, PricingMethod.Linear)]
        [InlineData(ParsedPricingMethod.Unknown, null)]
        public void ShouldCreateParsedOfferAccordingToPricingMethod(ParsedPricingMethod parsedPricingMethod, PricingMethod? pricingMethod)
        {
            var row = new OfferRow
            {
                PricingMethod = parsedPricingMethod,
                BillingMode = ParsedBillingMode.ActiveUsers,
                BillingUnit = ParsedBillingUnit.ActiveUsers,
                ForecastMethod = ParsedForecastMethod.AnnualCommitment,
                Category = "category",
                Currency = ParsedCurrency.EUR,
                Name = "offer",
                ProductName = "cleemy",
            };

            var product = new Product { Id = 1 };

            var parsedOffer = new ParsedOffer(row, product);

            parsedOffer.Should().NotBeNull();
            parsedOffer.PricingMethod.Should().Be(pricingMethod);
        }


        [Theory]
        [InlineData(ParsedBillingMode.ActiveUsers, BillingMode.ActiveUsers)]
        [InlineData(ParsedBillingMode.AllUsers, BillingMode.AllUsers)]
        [InlineData(ParsedBillingMode.FlatFee, BillingMode.FlatFee)]
        [InlineData(ParsedBillingMode.UsersWithAccess, BillingMode.UsersWithAccess)]
        [InlineData(ParsedBillingMode.Unknown, null)]
        public void ShouldCreateParsedOfferAccordingToBillingMode(ParsedBillingMode parsedBillingMode, BillingMode? billingMode)
        {
            var row = new OfferRow
            {
                PricingMethod = ParsedPricingMethod.Constant,
                BillingMode = parsedBillingMode,
                BillingUnit = ParsedBillingUnit.ActiveUsers,
                ForecastMethod = ParsedForecastMethod.AnnualCommitment,
                Category = "category",
                Currency = ParsedCurrency.EUR,
                Name = "offer",
                ProductName = "cleemy",
            };

            var product = new Product { Id = 1 };

            var parsedOffer = new ParsedOffer(row, product);

            parsedOffer.Should().NotBeNull();
            parsedOffer.BillingMode.Should().Be(billingMode);
        }

        [Theory]
        [InlineData(ParsedBillingUnit.ActiveUsers, BillingUnit.ActiveUsers)]
        [InlineData(ParsedBillingUnit.Cards, BillingUnit.Cards)]
        [InlineData(ParsedBillingUnit.Declarers, BillingUnit.Declarers)]
        [InlineData(ParsedBillingUnit.DownloadedDocuments, BillingUnit.DownloadedDocuments)]
        [InlineData(ParsedBillingUnit.FixedPrice, BillingUnit.FixedPrice)]
        [InlineData(ParsedBillingUnit.Licenses, BillingUnit.Licenses)]
        [InlineData(ParsedBillingUnit.Servers, BillingUnit.Servers)]
        [InlineData(ParsedBillingUnit.SynchronizedAccounts, BillingUnit.SynchronizedAccounts)]
        [InlineData(ParsedBillingUnit.Users, BillingUnit.Users)]
        [InlineData(ParsedBillingUnit.Unknown, null)]
        public void ShouldCreateParsedOfferAccordingToBillingUnit(ParsedBillingUnit parsedBillingUnit, BillingUnit? billingUnit)
        {
            var row = new OfferRow
            {
                PricingMethod = ParsedPricingMethod.Constant,
                BillingMode = ParsedBillingMode.FlatFee,
                BillingUnit = parsedBillingUnit,
                ForecastMethod = ParsedForecastMethod.AnnualCommitment,
                Category = "category",
                Currency = ParsedCurrency.EUR,
                Name = "offer",
                ProductName = "cleemy",
            };

            var product = new Product { Id = 1 };

            var parsedOffer = new ParsedOffer(row, product);

            parsedOffer.Should().NotBeNull();
            parsedOffer.BillingUnit.Should().Be(billingUnit);
        }

        [Theory]
        [InlineData(ParsedForecastMethod.AnnualCommitment, ForecastMethod.AnnualCommitment)]
        [InlineData(ParsedForecastMethod.LastRealMonth, ForecastMethod.LastRealMonth)]
        [InlineData(ParsedForecastMethod.LastYear, ForecastMethod.LastYear)]
        [InlineData(ParsedForecastMethod.Unknown, null)]
        public void ShouldCreateParsedOfferAccordingToForecastMethod(ParsedForecastMethod parsedForecastMethod, ForecastMethod? forecastMethod)
        {
            var row = new OfferRow
            {
                PricingMethod = ParsedPricingMethod.Constant,
                BillingMode = ParsedBillingMode.FlatFee,
                BillingUnit = ParsedBillingUnit.Users,
                ForecastMethod = parsedForecastMethod,
                Category = "category",
                Currency = ParsedCurrency.EUR,
                Name = "offer",
                ProductName = "cleemy",
            };

            var product = new Product { Id = 1 };

            var parsedOffer = new ParsedOffer(row, product);

            parsedOffer.Should().NotBeNull();
            parsedOffer.ForecastMethod.Should().Be(forecastMethod);
        }
    }
}
