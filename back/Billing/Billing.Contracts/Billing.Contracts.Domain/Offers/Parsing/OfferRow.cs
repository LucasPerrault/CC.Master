using System;

namespace Billing.Contracts.Domain.Offers.Parsing
{
    public class OfferRow
    {
        public string Name { get; set; }
        public int? ProductId { set; get; }
        public ParsedBillingUnit? BillingUnit { get; set; }
        public int? CurrencyId { get; set; }
        public string Category { get; set; }
        public ParsedBillingMode? BillingMode { get; set; }
        public ParsedPricingMethod? PricingMethod { get; set; }
        public ParsedForecastMethod? ForecastMethod { get; set; }
        public DateTime? StartsOn { get; set; }
        public int MinIncludedCount { get; set; }
        public int MaxIncludedCount { get; set; }
        public double UnitPrice { get; set; }
        public double FixedPrice { get; set; }
    }


    public enum ParsedBillingUnit
    {
        Unknown = 0,
        Users = 1,
        ActiveUsers = 2,
        Declarers = 3,
        Cards = 4,
        DownloadedDocuments = 5,
        SynchronizedAccounts = 6,
        Licenses = 7,
        Servers = 8,
        FixedPrice = 9
    }

    public enum ParsedPricingMethod
    {
        Unknown = 0,
        Constant = 1,
        Linear = 2,
        AnnualCommitment = 3,
    }

    public enum ParsedForecastMethod
    {
        Unknown = 0,
        LastRealMonth = 1,
        LastYear = 2,
        AnnualCommitment = 3,
    }

    public enum ParsedBillingMode
    {
        Unknown = 0,
        FlatFee = 1,
        AllUsers = 2,
        UsersWithAccess = 3,
        ActiveUsers = 4,
    }
}
