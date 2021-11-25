using Billing.Contracts.Domain.Offers.Interfaces;
using Billing.Contracts.Domain.Offers.Parsing;
using Billing.Products.Domain;
using Billing.Products.Domain.Interfaces;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using Lucca.Core.Shared.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billing.Contracts.Infra.Offers
{
    public static class HeaderRow
    {
        public const string Name = "Nom";
        public const string Product = "Produit";
        public const string BillingUnit = "Unité de décompte";
        public const string Currency = "Devise";
        public const string Tag = "Catégorisation";
        public const string BillingMode = "Mode de décompte";
        public const string PricingMethod = "Méthode de calcul";
        public const string ForecastMethod = "Méthode prévisionnelle";

        public const string ListStartsOn = "Date de début de la grille";
        public const string RowMin = "Borne inférieure";
        public const string RowMax = "Borne supérieure";
        public const string RowUnitPrice = "Prix unitaire";
        public const string RowFixedPrice = "Prix fixe";

        public const string LimitWarning = "xxxxx Ne rien écrire au-dessus de cette ligne";
    }

    public class OfferRowsService : IOfferRowsService
    {
        private static readonly CsvConfiguration _config = new CsvConfiguration(new CultureInfo("en-US", false)) // Culture set on en-US to get . as decimal separator
        {
            Delimiter = ",",
            HeaderValidated = null,
            PrepareHeaderForMatch = args => args.Header.ToLower(),
        };

        private readonly IProductsStore _productsStore;
        private readonly ParsedOffersService _parsedOffersService;

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

        public async Task<Stream> GetTemplateStreamAsync()
        {
            var csvBuilder = new CsvBuilder();
            await AddTemplateHelpAsync(csvBuilder);
            csvBuilder.AddCell(HeaderRow.LimitWarning).NewLine();
            AddTemplateWithExamples(csvBuilder.StringBuilder);
            return new MemoryStream(Encoding.UTF8.GetBytes(csvBuilder.ToString()));
        }

        private static void AddTemplateWithExamples(StringBuilder stringBuilder)
        {
            var writer = new CsvWriter(new StringWriter(stringBuilder), _config);
            writer.Context.RegisterClassMap<OfferRowMap>();
            writer.WriteHeader<OfferRow>();

            var cleemy = new OfferRow
            {
                Name = "Cleemy template 2021",
                ProductName = "Cleemy",
                BillingUnit = ParsedBillingUnit.ActiveUsers,
                Currency = ParsedCurrency.EUR,
                Category = "Catalogue",
                BillingMode = ParsedBillingMode.ActiveUsers,
                PricingMethod = ParsedPricingMethod.Linear,
                ForecastMethod = ParsedForecastMethod.LastRealMonth,
                StartsOn = new DateTime(2002, 01, 01),
                MinIncludedCount = 0,
                MaxIncludedCount = 10,
                UnitPrice = 0,
                FixedPrice = 50,
            };

            var figgo = new OfferRow
            {
                Name = "Figgo template 2021",
                ProductName = "Figgo",
                BillingUnit = ParsedBillingUnit.Users,
                Currency = ParsedCurrency.CHF,
                Category = "Catalogue",
                BillingMode = ParsedBillingMode.UsersWithAccess,
                PricingMethod = ParsedPricingMethod.Linear,
                ForecastMethod = ParsedForecastMethod.LastRealMonth,
                StartsOn = new DateTime(2002, 01, 01),
                MinIncludedCount = 0,
                MaxIncludedCount = 10,
                UnitPrice = 0,
                FixedPrice = 50,
            };

            writer.NextRecord();
            writer.WriteRecords(new List<OfferRow>
            {
                cleemy,
                TemplateExample.PriceRow(11, 20, 2, 0),
                TemplateExample.PriceRow(21, 50, 1.9m, 0),
                TemplateExample.PriceRow(51, 100, 1.5m, 0),
                TemplateExample.PriceRow(101, 1000, 1, 0),
                TemplateExample.PriceListStart(new DateTime(2024, 05, 01), 0, 10, 0, 30),
                TemplateExample.PriceRow(11, 20, 1.5m, 0),
                TemplateExample.PriceRow(21, 50, 1.4m, 0),
                TemplateExample.PriceRow(51, 100, 1, 0),
                TemplateExample.PriceRow(101, 1000, 0.5m, 0),
                figgo,
                TemplateExample.PriceRow(11, 20, 2, 0),
                TemplateExample.PriceRow(21, 50, 1.85m, 0),
                TemplateExample.PriceRow(51, 1000, 1.25m, 0),
                TemplateExample.PriceListStart(new DateTime(2025, 01, 01), 0, 10, 0, 30),
                TemplateExample.PriceRow(11, 20, 1.5m, 0),
                TemplateExample.PriceRow(21, 50, 1.4m, 0),
                TemplateExample.PriceRow(51, 1000, 1.2m, 0),
            });
        }

        private async Task AddTemplateHelpAsync(CsvBuilder csvBuilder)
        {
            var products = (await _productsStore.GetAsync(ProductsFilter.All, ProductsIncludes.All)).Select(p => p.Name).ToArray().ToCells();
            var billingUnit = GetAllEnumValuesExcept(ParsedBillingUnit.Unknown).ToCells();
            var billingMode = GetAllEnumValuesExcept(ParsedBillingMode.Unknown).ToCells();
            var forecastMethod = GetAllEnumValuesExcept(ParsedForecastMethod.Unknown).ToCells();
            var pricingMethod = GetAllEnumValuesExcept(ParsedPricingMethod.Unknown).ToCells();
            var currency = GetAllEnumValuesExcept(ParsedCurrency.Unknown).ToCells();

            var maxLength = products.Length
                .MaxLength(billingUnit)
                .MaxLength(billingMode)
                .MaxLength(forecastMethod)
                .MaxLength(pricingMethod)
                .MaxLength(currency);

            csvBuilder.AddCell(HeaderRow.Product)
                .AddCell(HeaderRow.BillingUnit)
                .AddCell(HeaderRow.Currency)
                .AddCell(HeaderRow.BillingMode)
                .AddCell(HeaderRow.PricingMethod)
                .AddCell(HeaderRow.ForecastMethod)
                .NewLine();


            for (var i = 0; i < maxLength; i++)
            {
                csvBuilder.AddCell(products.Get(i))
                    .AddCell(billingUnit.Get(i))
                    .AddCell(currency.Get(i))
                    .AddCell(billingMode.Get(i))
                    .AddCell(pricingMethod.Get(i))
                    .AddCell(forecastMethod.Get(i))
                    .NewLine();
            }

            // If you modify the number of lines writted by the following section, change RowGapBetweenMetadataAndTemplate value
            csvBuilder.NewLine();
        }

        private static string[] GetAllEnumValuesExcept<T>(params T[] exception) where T : Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>().Except(exception).Select(value => $"{value}").ToArray();
        }

        private class CsvBuilder
        {
            public StringBuilder StringBuilder { get; } = new StringBuilder();

            private bool _isCurrentLineEmpty = true;

            public CsvBuilder NewLine()
            {
                StringBuilder.AppendLine();
                _isCurrentLineEmpty = true;

                return this;
            }

            public CsvBuilder AddCell(string value)
            {
                if (!_isCurrentLineEmpty)
                {
                    StringBuilder.Append(',');
                }
                StringBuilder.Append(value);
                _isCurrentLineEmpty = false;

                return this;
            }

            public override string ToString()
            {
                return StringBuilder.ToString();
            }
        }
        private List<OfferRow> GetOfferRows(Stream stream, List<Product> products)
        {
            using var templateHelpDetector = new TemplateHelpDetector(stream);
            var containsTemplateHelp = templateHelpDetector.ContainsTemplateHelp();
            templateHelpDetector.RewindStream();

            using var reader = new StreamReader(stream);
            using var csv = new CsvReader(reader, _config);
            csv.Context.RegisterClassMap<OfferRowMap>();

            csv.Read();
            if (containsTemplateHelp)
            {
                while (!string.Equals(csv[0], HeaderRow.LimitWarning, StringComparison.CurrentCultureIgnoreCase))
                {
                    csv.Read();
                }
                csv.Read();
            }
            csv.ReadHeader();
            csv.ValidateHeader<OfferRow>();

            return csv.GetRecords<OfferRow>().ToList();
        }

        private class TemplateHelpDetector : StreamReader
        {
            public TemplateHelpDetector(Stream stream) : base(stream)
            {

            }

            public bool ContainsTemplateHelp()
            {
                string line;
                while (( line = ReadLine() ) is not null)
                {
                    if (line.StartsWith(HeaderRow.LimitWarning, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
                return false;
            }

            public void RewindStream()
            {
                BaseStream.Seek(0, SeekOrigin.Begin);
            }
        }
    }

    internal static class TemplateExample
    {
        public static OfferRow PriceRow(int min, int max, decimal unitPrice, decimal fixedPrice) => new OfferRow
        {
            MinIncludedCount = min,
            MaxIncludedCount = max,
            UnitPrice = unitPrice,
            FixedPrice = fixedPrice,
        };
        public static OfferRow PriceListStart(DateTime startsOn, int min, int max, decimal unitPrice, decimal fixedPrice) => new OfferRow
        {
            StartsOn = startsOn,
            MinIncludedCount = min,
            MaxIncludedCount = max,
            UnitPrice = unitPrice,
            FixedPrice = fixedPrice,
        };
    }

    internal static class OfferRowExtension
    {
        public static Cells ToCells(this string[] values) => new Cells(values);

        public static int MaxLength(this int max, Cells cells)
        {
            return Math.Max(cells.Length, max);
        }

        internal class Cells
        {
            private readonly string[] _values;

            public Cells(string[] values)
            {
                _values = values;
            }

            public string Get(int index)
            {
                if (_values.Length > index)
                {
                    return _values[index];
                }
                return string.Empty;
            }

            public int Length => _values.Length;
        }
    }
    public class OfferRowMap : ClassMap<OfferRow>
    {
        public OfferRowMap()
        {
            Map(o => o.Name).Name(HeaderRow.Name);
            Map(o => o.ProductName).Name(HeaderRow.Product);
            Map(o => o.BillingUnit).Name(HeaderRow.BillingUnit).TypeConverter(new NullableEnumConverter<ParsedBillingUnit>());
            Map(o => o.Currency).Name(HeaderRow.Currency).TypeConverter(new NullableEnumConverter<ParsedCurrency>());
            Map(o => o.Category).Name(HeaderRow.Tag);
            Map(o => o.BillingMode).Name(HeaderRow.BillingMode).TypeConverter(new NullableEnumConverter<ParsedBillingMode>());
            Map(o => o.PricingMethod).Name(HeaderRow.PricingMethod).TypeConverter(new NullableEnumConverter<ParsedPricingMethod>());
            Map(o => o.ForecastMethod).Name(HeaderRow.ForecastMethod).TypeConverter(new NullableEnumConverter<ParsedForecastMethod>());
            Map(o => o.StartsOn).Name(HeaderRow.ListStartsOn)
                .TypeConverterOption.Format("dd/MM/yyyy")
                .TypeConverterOption.Format("d/MM/yyyy")
                .TypeConverterOption.Format("dd/M/yyyy")
                .TypeConverterOption.Format("d/M/yyyy")
                .TypeConverter(new NullableDateTimeConverter());

            Map(o => o.MinIncludedCount).Name(HeaderRow.RowMin).TypeConverter<MandatoryInt32Converter>();
            Map(o => o.MaxIncludedCount).Name(HeaderRow.RowMax).TypeConverter<MandatoryInt32Converter>();
            Map(o => o.UnitPrice).Name(HeaderRow.RowUnitPrice).TypeConverter<MandatoryDecimalConverter>();
            Map(o => o.FixedPrice).Name(HeaderRow.RowFixedPrice).TypeConverter<MandatoryDecimalConverter>();
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

    internal class NullableDateTimeConverter : DateTimeConverter
    {
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            if (string.IsNullOrEmpty(text))
                return null;

            return base.ConvertFromString(text, row, memberMapData);
        }
    }


    internal class MandatoryInt32Converter : Int32Converter
    {
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            if (string.IsNullOrEmpty(text))
                throw new BadRequestException($"The field '{memberMapData.Names.First()}' is mandatory. Raw line : {row.Parser.RawRecord}");

            return base.ConvertFromString(text, row, memberMapData);
        }
    }

    internal class MandatoryDecimalConverter : DecimalConverter
    {
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            if (string.IsNullOrEmpty(text))
                throw new BadRequestException($"The field '{memberMapData.Names.First()}' is mandatory. Raw line : {row.Parser.RawRecord}");

            return base.ConvertFromString(text, row, memberMapData);
        }
    }
}
