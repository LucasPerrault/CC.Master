using Billing.Products.Domain;
using System;
using System.Collections.Generic;

namespace Billing.Contracts.Domain.Offers.Parsing
{
    public class ParsedOffer
    {
        public string Name { get; set; }
        public Product Product { set; get; }
        public BillingUnit? BillingUnit { get; set; }
        public int? CurrencyId { get; set; }
        public string Category { get; set; }
        public BillingMode? BillingMode { get; set; }
        public PricingMethod? PricingMethod { get; set; }
        public ForecastMethod? ForecastMethod { get; set; }
        public List<ParsedPriceList> PriceLists { get; set; } = new List<ParsedPriceList>();

        public ParsedOffer(OfferRow row, Product product)
        {
            Name = row.Name;
            Product = product;
            BillingMode = GetBillingMode(row.BillingMode);
            CurrencyId = (int?)row.Currency;
            BillingUnit = GetBillingUnit(row.BillingUnit);
            PricingMethod = GetPricingMethod(row.PricingMethod);
            Category = row.Category;
            ForecastMethod = GetForecastMethod(row.ForecastMethod);
        }


        private static BillingMode? GetBillingMode(ParsedBillingMode? billingMode) => billingMode switch
        {
            ParsedBillingMode.ActiveUsers => Offers.BillingMode.ActiveUsers,
            ParsedBillingMode.AllUsers => Offers.BillingMode.AllUsers,
            ParsedBillingMode.FlatFee => Offers.BillingMode.FlatFee,
            ParsedBillingMode.UsersWithAccess => Offers.BillingMode.UsersWithAccess,
            _ => null,
        };

        private static BillingUnit? GetBillingUnit(ParsedBillingUnit? billingMode) => billingMode switch
        {
            ParsedBillingUnit.ActiveUsers => Offers.BillingUnit.ActiveUsers,
            ParsedBillingUnit.Cards => Offers.BillingUnit.Cards,
            ParsedBillingUnit.Declarers => Offers.BillingUnit.Declarers,
            ParsedBillingUnit.DownloadedDocuments => Offers.BillingUnit.DownloadedDocuments,
            ParsedBillingUnit.FixedPrice => Offers.BillingUnit.FixedPrice,
            ParsedBillingUnit.Licenses => Offers.BillingUnit.Licenses,
            ParsedBillingUnit.Servers => Offers.BillingUnit.Servers,
            ParsedBillingUnit.SynchronizedAccounts => Offers.BillingUnit.SynchronizedAccounts,
            ParsedBillingUnit.Users => Offers.BillingUnit.Users,
            _ => null,
        };

        private static ForecastMethod? GetForecastMethod(ParsedForecastMethod? forecastMethod) => forecastMethod switch
        {
            ParsedForecastMethod.AnnualCommitment => Offers.ForecastMethod.AnnualCommitment,
            ParsedForecastMethod.LastRealMonth => Offers.ForecastMethod.LastRealMonth,
            ParsedForecastMethod.LastYear => Offers.ForecastMethod.LastYear,
            _ => null,
        };

        private static PricingMethod? GetPricingMethod(ParsedPricingMethod? pricingMethod) => pricingMethod switch
        {
            ParsedPricingMethod.AnnualCommitment => Offers.PricingMethod.AnnualCommitment,
            ParsedPricingMethod.Constant => Offers.PricingMethod.Constant,
            ParsedPricingMethod.Linear => Offers.PricingMethod.Linear,
            _ => null,
        };
    }


    public class ParsedPriceList
    {
        public DateTime? StartsOn { get; set; }
        public List<ParsedPriceRow> Rows { get; set; }
    }

    public class ParsedPriceRow
    {
        public int? MaxIncludedCount { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? FixedPrice { get; set; }
    }
}
