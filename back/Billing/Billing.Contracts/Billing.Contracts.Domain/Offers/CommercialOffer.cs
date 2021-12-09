using Billing.Products.Domain;
using System;
using System.Collections.Generic;

namespace Billing.Contracts.Domain.Offers
{
    public enum BillingUnit
    {
        None = 0,
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

    public enum PricingMethod
    {
        Constant = 1,
        Linear = 2,
        AnnualCommitment = 3,
    }

    public enum ForecastMethod
    {
        LastRealMonth = 1,
        LastYear = 2,
        AnnualCommitment = 3,
    }

    public enum BillingMode
    {
        FlatFee = 1,
        AllUsers = 2,
        UsersWithAccess = 3,
        ActiveUsers = 4,
    }

    public class CommercialOffer
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public BillingMode BillingMode { get; set; }
        public BillingUnit Unit { get; set; }
        public PricingMethod PricingMethod { get; set; }
        public ForecastMethod ForecastMethod { get; set; }

        public string Tag { get; set; }

        public int CurrencyId { get; set; }

        public bool IsArchived { get; set; }

        public List<PriceList> PriceLists { get; set; }
    }

    public class PriceList
    {
        public int Id { get; set; }
        public int OfferId { get; set; }
        public DateTime StartsOn { get; set; }
        public List<PriceRow> Rows { get; set; }
    }

    public class PriceRow
    {
        public int Id { get; set; }
        public int ListId { get; set; }
        public int MaxIncludedCount { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal FixedPrice { get; set; }
    }
}
