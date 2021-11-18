using CsvHelper.Configuration.Attributes;

namespace Billing.Contracts.Application.Offers.Dtos
{
    public class OfferRow
    {
        [Name("Nom")]
        public string Name { get; set; }
        [Name("Produit")]
        public int? ProductId { set; get; }
        [Name("Unite de decompte")]
        public string BillingUnit { get; set; }
        [Name("Id devise")]
        public int? CurrencyID { get; set; }
        [Name("Categorisation")]
        public string Category { get; set; }
        [Name("Mode Decompte")]
        public string BillingMode { get; set; }
        [Name("Methode de pricing")]
        public string PricingMethod { get; set; }
        [Name("Algorithme previsionnel")]
        public string ForecastMethod { get; set; }

        [Name("Date de debut de la grille")]
        public string StartsOn { get; set; }

        [Name("Borne inferieure")]
        public int? MinIncludedCount { get; set; }

        [Name("Borne superieure")]
        public int? MaxIncludedCount { get; set; }

        [Name("Prix unitaire")]
        public double? UnitPrice { get; set; }

        [Name("Prix forfaitaire")]
        public double? FixedPrice { get; set; }

    }
}
