using Billing.Contracts.Domain.Offers.Parsing;
using Billing.Products.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools;

namespace Billing.Contracts.Infra.Offers
{
    public class TemplateHelper
    {
        public static void AddTemplateHelp(StringBuilder builder, List<Product> products)
        {
            var csvBuilder = new CsvBuilder(builder);
            var productNames = products.Select(p => p.Name).ToArray().ToCells();
            var billingUnit = EnumExtensions.GetAllEnumValuesExcept(ParsedBillingUnit.Unknown).ToCells();
            var billingMode = EnumExtensions.GetAllEnumValuesExcept(ParsedBillingMode.Unknown).ToCells();
            var forecastMethod = EnumExtensions.GetAllEnumValuesExcept(ParsedForecastMethod.Unknown).ToCells();
            var pricingMethod = EnumExtensions.GetAllEnumValuesExcept(ParsedPricingMethod.Unknown).ToCells();
            var currency = EnumExtensions.GetAllEnumValuesExcept(ParsedCurrency.Unknown).ToCells();

            var maxLength = productNames.Length
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
                csvBuilder.AddCell(productNames.Get(i))
                    .AddCell(billingUnit.Get(i))
                    .AddCell(currency.Get(i))
                    .AddCell(billingMode.Get(i))
                    .AddCell(pricingMethod.Get(i))
                    .AddCell(forecastMethod.Get(i))
                    .NewLine();
            }

            csvBuilder.NewLine();
        }

        private class CsvBuilder
        {
            private readonly StringBuilder _stringBuilder;
            private bool _isCurrentLineEmpty = true;

            public CsvBuilder(StringBuilder stringBuilder)
            {
                _stringBuilder = stringBuilder;
            }

            public CsvBuilder NewLine()
            {
                _stringBuilder.AppendLine();
                _isCurrentLineEmpty = true;

                return this;
            }

            public CsvBuilder AddCell(string value)
            {
                if (!_isCurrentLineEmpty)
                {
                    _stringBuilder.Append(',');
                }

                _stringBuilder.Append(value);
                _isCurrentLineEmpty = false;

                return this;
            }

            public override string ToString()
            {
                return _stringBuilder.ToString();
            }
        }

        public static List<OfferRow> GetExamples()
        {
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

            return new List<OfferRow>
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
            };
        }
    }
}
