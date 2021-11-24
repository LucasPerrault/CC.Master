using Billing.Contracts.Domain.Offers.Interfaces;
using Billing.Contracts.Domain.Offers.Parsing;
using Billing.Products.Domain;
using Billing.Products.Domain.Interfaces;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billing.Contracts.Infra.Offers
{

    public class OfferRowsService : IOfferRowsService
    {
        private readonly IProductsStore _productsStore;
        private readonly ParsedOffersService _parsedOffersService;

        private const int RowGapBetweenMetadataAndTemplate = 3;
        public OfferRowsService(IProductsStore productsStore, ParsedOffersService parsedOffersService)
        {
            _productsStore = productsStore;
            _parsedOffersService = parsedOffersService;
        }
        public async Task<List<ParsedOffer>> UploadAsync(Stream stream)
        {
            var products = await _productsStore.GetAsync(ProductsFilter.All, new ProductsIncludes());

            var offerRows = GetOfferRows(stream, products);

            var importedOffers = _parsedOffersService.ConvertToParsedOffers(offerRows, products);

            return importedOffers;
        }

        public async Task<MemoryStream> GetTemplateStreamAsync()
        {
            var products = await _productsStore.GetAsync(ProductsFilter.All, ProductsIncludes.All);
            var billingUnit = Enum.GetNames(typeof(ParsedBillingUnit));
            var billingMode = Enum.GetNames(typeof(ParsedBillingMode));
            var forecastMethod = Enum.GetNames(typeof(ParsedForecastMethod));
            var pricingMethod = Enum.GetNames(typeof(ParsedPricingMethod));
            var currency = Enum.GetNames(typeof(ParsedCurrency));

            var s = new StringBuilder();

            s.AppendLine("Id produit,Produit,Unite de decompte,Devise,Mode Decompte,Methode de pricing,Algorithme previsionnel");

            for (var i = 0;  i < products.Count;  i++)
            {
                s.Append($"{products[i].Id},{products[i].Name},");
                s.Append(billingUnit.Length > i + 1 ? billingUnit[i + 1] : string.Empty);
                s.Append(',');
                s.Append(currency.Length > i + 1 ? currency[i + 1] : string.Empty);
                s.Append(',');
                s.Append(billingMode.Length > i + 1 ? billingMode[i + 1] : string.Empty);
                s.Append(',');
                s.Append(pricingMethod.Length > i + 1 ? pricingMethod[i + 1] : string.Empty);
                s.Append(',');
                s.AppendLine(forecastMethod.Length > i + 1 ? forecastMethod[i + 1] : string.Empty);
            }

            // If you modify the number of lines writted by the following section, change RowGapBetweenMetadataAndTemplate value
            s.AppendLine();
            s.AppendLine("xxxxx Ne rien écrire au-dessus de cette ligne");
            s.AppendLine();

            s.AppendLine("Nom,Id produit,Unite de decompte,devise,Categorisation,Mode Decompte,Methode de pricing,Algorithme previsionnel,Date de debut de la grille,Borne inferieure,Borne superieure,Prix unitaire,Prix forfaitaire");
            s.AppendLine("Cleemy template 2021,2,1,EUR,catalogues,3,Linear,LastRealMonth,01/12/2021,0,10,0,50");
            s.AppendLine(",,,,,,,,,11,20,2,0");
            s.AppendLine(",,,,,,,,,21,50,1.9,0");
            s.AppendLine(",,,,,,,,,51,100,1.5,0");
            s.AppendLine(",,,,,,,,,101,1000,1,0");
            s.AppendLine(",,,,,,,,01/08/2020,0,10,0,30");
            s.AppendLine(",,,,,,,,,11,20,1.5,0");
            s.AppendLine(",,,,,,,,,21,50,1.4,0");
            s.AppendLine(",,,,,,,,,51,100,1,0");
            s.AppendLine(",,,,,,,,,101,1000,0.5,0");

            s.AppendLine("Figgo template 2021,1,1,CHF,catalogues,3,Linear,LastRealMonth,01/12/2021,0,10,0,50");
            s.AppendLine(",,,,,,,,,11,20,2,0");
            s.AppendLine(",,,,,,,,,21,50,1.9,0");
            s.AppendLine(",,,,,,,,,51,100,1.5,0");
            s.AppendLine(",,,,,,,,,101,1000,1,0");
            s.AppendLine(",,,,,,,,01/08/2020,0,10,0,30");
            s.AppendLine(",,,,,,,,,11,20,1.5,0");
            s.AppendLine(",,,,,,,,,21,50,1.4,0");
            s.AppendLine(",,,,,,,,,51,100,1,0");
            s.AppendLine(",,,,,,,,,101,1000,0.5,0");
            return new MemoryStream(Encoding.UTF8.GetBytes(s.ToString()));
        }
        private List<OfferRow> GetOfferRows(Stream stream, List<Product> products)
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

            csv.Read();
            csv.ReadHeader();
            if (csv[0].ToLower() == "id produit")
            {
                for (var i = 0; i < products.Count + RowGapBetweenMetadataAndTemplate - 1; i++)
                    csv.Read();
                csv.ReadHeader();
            }

            csv.ValidateHeader<OfferRow>();

            return csv.GetRecords<OfferRow>().ToList();
        }
    }

    public class OfferRowMap : ClassMap<OfferRow>
    {
        public OfferRowMap()
        {
            Map(o => o.Name).Name("nom");
            Map(o => o.ProductId).Name("id produit");
            Map(o => o.BillingUnit).Name("unite de decompte", "unité de décompte").TypeConverter(new NullableEnumConverter<ParsedBillingUnit>());
            Map(o => o.Currency).Name("devise").TypeConverter(new NullableEnumConverter<ParsedCurrency>());
            Map(o => o.Category).Name("categorisation");
            Map(o => o.BillingMode).Name("mode decompte", "mode décompte").TypeConverter(new NullableEnumConverter<ParsedBillingMode>());
            Map(o => o.PricingMethod).Name("methode de pricing", "méthode de pricing").TypeConverter(new NullableEnumConverter<ParsedPricingMethod>());
            Map(o => o.ForecastMethod).Name("algorithme previsionnel", "algorithme prévisionnel").TypeConverter(new NullableEnumConverter<ParsedForecastMethod>());
            Map(o => o.StartsOn).Name("date de début de la grille", "date de debut de la grille")
                .TypeConverterOption.Format("dd/MM/yyyy")
                .TypeConverterOption.Format("d/MM/yyyy")
                .TypeConverterOption.Format("dd/M/yyyy")
                .TypeConverterOption.Format("d/M/yyyy");

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
