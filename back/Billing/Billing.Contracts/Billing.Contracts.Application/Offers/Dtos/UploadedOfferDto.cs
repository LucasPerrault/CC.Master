using System.Collections.Generic;

namespace Billing.Contracts.Application.Offers.Dtos
{
    public class UploadedOfferDto
    {
        public string Name { get; set; }
        public int? ProductId { set; get; }
        public string BillingUnit { get; set; }
        public int? CurrencyID { get; set; }
        public string Category { get; set; }
        public string BillingMode { get; set; }
        public string PricingMethod { get; set; }
        public string ForecastMethod { get; set; }
        public List<ImportedPriceList> PriceLists { get; set; } = new List<ImportedPriceList>();

        public UploadedOfferDto(OfferRow row)
        {
            Name = row.Name;
            ProductId = row.ProductId.Value;
            BillingMode = row.BillingMode;
            CurrencyID = row.CurrencyID.Value;
            BillingUnit = row.BillingUnit;
            PricingMethod = row.PricingMethod;
            Category = row.Category;
            ForecastMethod = row.ForecastMethod;
        }
    }
}
