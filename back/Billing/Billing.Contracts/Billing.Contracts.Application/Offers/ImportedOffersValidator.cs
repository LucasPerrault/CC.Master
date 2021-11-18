using Billing.Contracts.Application.Offers.Dtos;
using Billing.Contracts.Application.Offers.Exceptions;
using Billing.Contracts.Domain.Offers;
using CsvHelper;
using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Billing.Contracts.Application.Offers
{
    public static class ImportedOffersValidator
    {
        public static void EnsureHeaderIsCorrect(CsvReader csv)
        {
            var errors = new List<string>();

            var expectedHeaders = GetExpectedHeaders();
            var currentHeaders = csv.HeaderRecord;

            if (expectedHeaders.Count != currentHeaders.Length)
            {
                var missingHeaders = expectedHeaders.Except(currentHeaders);
                var notExpectedHeaders = currentHeaders.Except(expectedHeaders);

                foreach (var missingheader in missingHeaders)
                    errors.Add($"The following header is missing : {missingheader}");

                foreach (var notExpectedHeader in notExpectedHeaders)
                    errors.Add($"The following header is not expected : {notExpectedHeader}");
            }
            else
            {
                for (var i = 0; i < expectedHeaders.Count; i++)
                {
                    if (expectedHeaders[i] != currentHeaders[i])
                        errors.Add($"Expecting header is {expectedHeaders[i]} but found {currentHeaders[i]}");
                }
            }

            if (errors.Any())
                throw new ImportOffersHeaderException(string.Join("," + Environment.NewLine, errors.ToArray()));
        }

        private static List<string> GetExpectedHeaders()
        {
            var headers = new List<string>();

            var props = typeof(OfferRow).GetProperties();
            foreach (var prop in props)
            {
                var attrs = prop.GetCustomAttributes(true);
                foreach (var attr in attrs)
                {
                    if (attr is NameAttribute nameAttributes)
                        headers.Add(nameAttributes.Names.Single());
                }
            }

            return headers;
        }


        public static void EnsureNewPriceRowIsCorrect(OfferRow row)
        {
            var errors = new List<string>();

            if (row.MaxIncludedCount is null) errors.Add("MaxIncludedCount is mandatory");
            if (row.MinIncludedCount is null) errors.Add("MinIncludedCount is mandatory");
            if (row.FixedPrice is null) errors.Add("FixedPrice is mandatory");
            if (row.UnitPrice is null) errors.Add("UnitPrice is mandatory");

            if (errors.Any())
                throw new ImportOffersNewOfferRowException(string.Join("," + Environment.NewLine, errors.ToArray()));
        }

        public static void EnsureNewPriceListIsCorrect(OfferRow row)
        {
            if (row.BillingUnit is null)

                if (row.StartsOn is null || !DateTime.TryParse(row.StartsOn, out var startOn) || startOn == default)
                    throw new ImportOffersNewPriceListException("StartOn must be specified with format 'dd/mm/yyyy'");
        }

        public static void EnsureNewOfferIsCorrect(OfferRow row)
        {
            var errors = new List<string>();

            if (row.BillingUnit is null) errors.Add("Billing unit is mandatory");
            if (row.CurrencyID is null) errors.Add("Currency id is mandatory");
            if (row.ProductId is null) errors.Add("Product id is mandatory");

            if (string.IsNullOrWhiteSpace(row.Name)) errors.Add("Name cannot be null or white space");

            if (string.IsNullOrWhiteSpace(row.PricingMethod) || !Enum.IsDefined(typeof(PricingMethod), row.PricingMethod))
                errors.Add($"PricingMethod must be between {string.Join(",", Enum.GetNames(typeof(PricingMethod)))}");

            if (string.IsNullOrWhiteSpace(row.BillingMode) || !Enum.IsDefined(typeof(BillingMode), row.BillingMode))
                errors.Add($"BillingMode must be between {string.Join(",", Enum.GetNames(typeof(BillingMode)))}");

            if (string.IsNullOrWhiteSpace(row.ForecastMethod) || !Enum.IsDefined(typeof(ForecastMethod), row.ForecastMethod))
                errors.Add($"ForecastMethod must be between {string.Join(",", Enum.GetNames(typeof(ForecastMethod)))}");

            if (errors.Any())
                throw new ImportOffersNewOfferRowException(string.Join("," + Environment.NewLine, errors.ToArray()));
        }
    }
}
