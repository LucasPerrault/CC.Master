using Billing.Contracts.Domain.Offers;
using Billing.Products.Domain;
using System;
using System.Collections.Generic;

namespace Billing.Contracts.Application.Offers.Dtos
{
    public class ImportedOfferDto
    {
        public string Name { get; set; }
        public Product Product { set; get; }
        public BillingUnit? BillingUnit { get; set; }
        public int? CurrencyID { get; set; }
        public string Category { get; set; }
        public BillingMode? BillingMode { get; set; }
        public PricingMethod? PricingMethod { get; set; }
        public ForecastMethod? ForecastMethod { get; set; }
        public List<ImportedPriceList> PriceLists { get; set; } = new List<ImportedPriceList>();

        public ImportedOfferDto(UploadedOfferDto uploadedOffer, Product product)
        {
            Name = uploadedOffer.Name;
            Product = product;
            CurrencyID = uploadedOffer.CurrencyID;
            Category = uploadedOffer.Category;

            if (Enum.TryParse<BillingMode>(uploadedOffer.BillingMode, out var billingMode))
                BillingMode = billingMode;

            if (Enum.TryParse<BillingUnit>(uploadedOffer.BillingUnit, out var billingUnit))
                BillingUnit = billingUnit;

            if (Enum.TryParse<PricingMethod>(uploadedOffer.PricingMethod, out var pricingMethod))
                PricingMethod = pricingMethod;

            if (Enum.TryParse<ForecastMethod>(uploadedOffer.ForecastMethod, out var forecastMethod))
                ForecastMethod = forecastMethod;

            PriceLists = uploadedOffer.PriceLists;
        }
    }

    public class ImportedPriceList
    {
        public DateTime? StartsOn { get; set; }
        public List<ImportedPriceRow> Rows { get; set; }
    }

    public class ImportedPriceRow
    {
        public int? MaxIncludedCount { get; set; }
        public double? UnitPrice { get; set; }
        public double? FixedPrice { get; set; }
    }
}
