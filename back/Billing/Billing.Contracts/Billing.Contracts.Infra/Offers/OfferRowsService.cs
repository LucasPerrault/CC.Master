using Billing.Contracts.Domain.Offers.Interfaces;
using Billing.Contracts.Domain.Offers.Parsing;
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
            PrepareHeaderForMatch = args => args.Header.ToLower(CultureInfo.InvariantCulture),
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
            var offerRows = GetOfferRows(stream);
            var importedOffers = _parsedOffersService.ConvertToParsedOffers(offerRows, products);
            return importedOffers;
        }

        public async Task<Stream> GetTemplateStreamAsync()
        {
            var builder = new StringBuilder();
            var products = await _productsStore.GetAsync(ProductsFilter.All, ProductsIncludes.All);
            TemplateHelper.AddTemplateHelp(builder, products);
            builder.AppendLine();
            builder.AppendLine(HeaderRow.LimitWarning);
            AddTemplateWithExamples(builder);
            return new MemoryStream(Encoding.UTF8.GetBytes(builder.ToString()));
        }

        private static void AddTemplateWithExamples(StringBuilder stringBuilder)
        {
            var writer = new CsvWriter(new StringWriter(stringBuilder), _config);
            writer.Context.RegisterClassMap<OfferRowMap>();
            writer.WriteHeader<OfferRow>();
            writer.NextRecord();
            writer.WriteRecords(TemplateHelper.GetExamples());
        }

        private static List<OfferRow> GetOfferRows(Stream stream)
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
