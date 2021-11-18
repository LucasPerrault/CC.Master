using Billing.Contracts.Domain.Offers.Interfaces;
using Billing.Contracts.Domain.Offers.Parsing;
using Billing.Products.Domain.Interfaces;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Billing.Contracts.Infra.Offers
{

    public class OfferRowsService : IOfferRowsService
    {
        private readonly IProductsStore _productsStore;
        private readonly ParsedOffersService _parsedOffersService;

        public OfferRowsService(IProductsStore productsStore, ParsedOffersService parsedOffersService)
        {
            _productsStore = productsStore;
            _parsedOffersService = parsedOffersService;
        }
        public async Task<List<ParsedOffer>> UploadAsync(Stream stream)
        {
            var offerRows = GetOfferRows(stream);

            var products = await _productsStore.GetAsync(ProductsFilter.All, new ProductsIncludes());

            var importedOffers = _parsedOffersService.ConvertToParsedOffers(offerRows, products);

            return importedOffers;
        }
        // TODO ajouter une méthode pour récupérer le fichier template qu'on créera avec CSV Writer


        private static List<OfferRow> GetOfferRows(Stream stream)
        {
            var config = new CsvConfiguration(new CultureInfo("en-US", false)) // Culture set on en-US to get . as decimal separator
            {
                Delimiter = ",",
                HeaderValidated = null,
                PrepareHeaderForMatch = args => args.Header.ToLower(),
            };

            using var reader = new StreamReader(stream);
            using var csv = new CsvReader(reader, config);

            csv.Context.RegisterClassMap<OfferRowMap>();

            return csv.GetRecords<OfferRow>().ToList();
        }
    }

    public class OfferRowMap : ClassMap<OfferRow>
    {
        public OfferRowMap()
        {
            Map(o => o.Name).Name("nom");
            Map(o => o.ProductId).Name("produit");
            Map(o => o.BillingUnit).Name("unite de decompte", "unité de décompte").TypeConverter(new NullableEnumConverter<ParsedBillingUnit>());
            Map(o => o.CurrencyId).Name("id devise");
            Map(o => o.Category).Name("categorisation");
            Map(o => o.BillingMode).Name("mode decompte", "mode décompte").TypeConverter(new NullableEnumConverter<ParsedBillingMode>());
            Map(o => o.PricingMethod).Name("methode de pricing", "méthode de pricing").TypeConverter(new NullableEnumConverter<ParsedPricingMethod>());
            Map(o => o.ForecastMethod).Name("algorithme previsionnel", "algorithme prévisionnel").TypeConverter(new NullableEnumConverter<ParsedForecastMethod>());
            Map(o => o.StartsOn).Name("date de début de la grille", "date de debut de la grille").TypeConverterOption.Format("dd/MM/yyyy");
            Map(o => o.MinIncludedCount).Name("borne inferieure", "borne inférieure");
            Map(o => o.MaxIncludedCount).Name("borne superieure", "borne supérieure");
            Map(o => o.UnitPrice).Name("prix unitaire");
            Map(o => o.FixedPrice).Name("prix forfaitaire");
        }
    }

    public class NullableEnumConverter<T> : EnumConverter where T : struct
    {
        public NullableEnumConverter() : base(typeof(T))
        {

        }
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            if (string.IsNullOrEmpty(text))
                return null;

            try
            {
                return base.ConvertFromString(text, row, memberMapData);
            }
            catch
            {
                return null;
            }
        }
    }
}
