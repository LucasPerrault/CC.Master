using Billing.Contracts.Application.Offers.Dtos;
using Billing.Products.Domain.Interfaces;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Billing.Contracts.Application.Offers
{
    public interface IImportedOffersService
    {
        Task<IEnumerable<ImportedOfferDto>> UploadAsync(IFormFile file);
    }

    public class ImportedOffersService : IImportedOffersService
    {
        private readonly IProductsStore _productsStore;

        public ImportedOffersService(IProductsStore productsStore)
        {
            _productsStore = productsStore;
        }
        public async Task<IEnumerable<ImportedOfferDto>> UploadAsync(IFormFile file)
        {
            var uploadedOffers = await GetUploadedOffersAsync(file);

            return await ConvertToImportedOffersAsync(uploadedOffers);
        }

        private static async Task<List<UploadedOfferDto>> GetUploadedOffersAsync(IFormFile file)
        {
            var config = new CsvConfiguration(new CultureInfo("en-US", false)) // Culture set on en-US to get . as decimal separator
            {
                Delimiter = ",",
                HeaderValidated = null,

            };

            using (var ms = new MemoryStream())
            {
                await file.CopyToAsync(ms);
                ms.Position = 0;
                using (var reader = new StreamReader(ms))
                using (var csv = new CsvReader(reader, config))
                {
                    csv.Read();
                    csv.ReadHeader();
                    ImportedOffersValidator.EnsureHeaderIsCorrect(csv);

                    return ReadFile(csv);
                }
            }
        }

        

        
        private static List<UploadedOfferDto> ReadFile(CsvReader csv)
        {
            var uploadedOffers = new List<UploadedOfferDto>();

            UploadedOfferDto currentOffer = null;
            ImportedPriceList currentPriceList = null;

            while (csv.Read())
            {
                var row = csv.GetRecord<OfferRow>();

                var isNewOffer = !string.IsNullOrEmpty(row.Name) || !string.IsNullOrEmpty(row.BillingMode) || row.ProductId.HasValue || !string.IsNullOrEmpty(row.PricingMethod) || !string.IsNullOrEmpty(row.BillingUnit);
                if (isNewOffer)
                {
                    //ImportedOffersValidator.EnsureNewOfferIsCorrect(row);
                    currentOffer = new UploadedOfferDto(row);
                    uploadedOffers.Add(currentOffer);
                }

                var isNewPriceList = !string.IsNullOrEmpty(row.StartsOn);

                //if(isNewPriceList)
                //    ImportedOffersValidator.EnsureNewPriceListIsCorrect(row);

                //ImportedOffersValidator.EnsureNewPriceRowIsCorrect(row);

                var priceRow = new ImportedPriceRow
                {
                    MaxIncludedCount = row.MaxIncludedCount.Value,
                    FixedPrice = row.FixedPrice.Value,
                    UnitPrice = row.UnitPrice.Value
                };

                if (isNewPriceList)
                {
                    DateTime? startOn = DateTime.TryParse(row.StartsOn, out var date) ? date : null;
                    currentPriceList = new ImportedPriceList
                    {
                        StartsOn = startOn,
                        Rows = new List<ImportedPriceRow> { priceRow }
                    };


                    currentOffer.PriceLists.Add(currentPriceList);
                }
                else
                    currentPriceList.Rows.Add(priceRow);
            }

            return uploadedOffers;
        }

        private async Task<IEnumerable<ImportedOfferDto>> ConvertToImportedOffersAsync(List<UploadedOfferDto> uploadedOffers)
        {
            var products = await _productsStore.GetAsync(ProductsFilter.All, new ProductsIncludes());

            return uploadedOffers.Select(o => new ImportedOfferDto(o, products.SingleOrDefault(p => o.ProductId == p.Id)));
        }

        
    }
}
