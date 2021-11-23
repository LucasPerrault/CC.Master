using Billing.Contracts.Application.Offers;
using Billing.Contracts.Domain.Offers;
using Billing.Contracts.Domain.Offers.Filtering;
using Billing.Contracts.Domain.Offers.Interfaces;
using Billing.Contracts.Domain.Offers.Parsing;
using Lucca.Core.Api.Abstractions.Paging;
using Lucca.Core.Api.Web.ModelBinding.Sorting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rights.Domain;
using Rights.Web.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Tools.Web;
using Billing.Contracts.Application.Offers.Dtos;
using Billing.Contracts.Domain.Offers.Interfaces;
using Billing.Contracts.Domain.Offers.Filtering;
using Lucca.Core.Api.Abstractions.Paging;
using Microsoft.AspNetCore.Http;

namespace Billing.Contracts.Web
{
    [ApiController, Route("/api/commercial-offers")]
    [ApiSort(nameof(CommercialOffer.Name))]
    public class CommercialOffersController
    {
        private readonly CommercialOffersRepository _commercialOffersRepository;
        private readonly IOfferRowsService _importedOfferService;

        public CommercialOffersController(CommercialOffersRepository commercialOffersRepository, IOfferRowsService importedOfferService)
        {
            _commercialOffersRepository = commercialOffersRepository;
            _importedOfferService = importedOfferService;
        }


        [HttpGet]
        [ForbidIfMissing(Operation.ReadCommercialOffers)]
        public Task<Page<CommercialOffer>> GetAsync([FromQuery] CommercialOfferQuery query)
        {
            return _commercialOffersRepository.GetPageAsync(query.PageToken, query.ToFilter());
        }

        [HttpGet("{id:int}")]
        [ForbidIfMissing(Operation.ReadCommercialOffers)]
        public Task<CommercialOffer> GetByIdAsync([FromRoute] int id)
        {
            return _commercialOffersRepository.GetByIdAsync(id);
        }

        [HttpGet("tags")]
        [ForbidIfMissing(Operation.ReadCommercialOffers)]
        public Task<Page<string>> GetTagsAsync()
        {
            return _commercialOffersRepository.GetTagsAsync();
        }

        [HttpPost]
        [ForbidIfMissing(Operation.CreateCommercialOffers)]
        public Task<CommercialOffer> CreateAsync([FromBody] CommercialOffer body)
        {
            return _commercialOffersRepository.CreateAsync(body);
        }

        [HttpPost("creation")]
        [ForbidIfMissing(Operation.CreateCommercialOffers)]
        public Task<IReadOnlyCollection<CommercialOffer>> CreateManyAsync([FromBody] IReadOnlyCollection<CommercialOffer> body)
        {
            return _commercialOffersRepository.CreateManyAsync(body);
        }

        [HttpPut("{id:int}")]
        [ForbidIfMissing(Operation.CreateCommercialOffers)]
        public Task PutAsync([FromRoute] int id, [FromBody] CommercialOffer body)
        {
            return _commercialOffersRepository.PutAsync(id, body);
        }

        [HttpDelete("{id:int}")]
        [ForbidIfMissing(Operation.CreateCommercialOffers)]
        public Task DeleteAsync([FromRoute] int id)
        {
            return _commercialOffersRepository.DeleteAsync(id);
        }

        [HttpPost("{id:int}/price-lists")]
        [ForbidIfMissing(Operation.CreateCommercialOffers)]
        public Task<CommercialOffer> AddPriceListAsync([FromRoute] int id, [FromBody] PriceList body)
        {
            return _commercialOffersRepository.AddPriceListAsync(id, body);
        }

        [HttpPut("{id:int}/price-lists/{listId:int}")]
        [ForbidIfMissing(Operation.CreateCommercialOffers)]
        public Task ModifyPriceListAsync([FromRoute] int id, [FromRoute] int listId, [FromBody] PriceList body)
        {
            return _commercialOffersRepository.ModifyPriceListAsync(id, listId, body);
        }

        [HttpDelete("{id:int}/price-lists/{listId:int}")]
        [ForbidIfMissing(Operation.CreateCommercialOffers)]
        public Task DeletePriceListAsync([FromRoute] int id, [FromRoute] int listId)
        {
            return _commercialOffersRepository.DeletePriceListAsync(id, listId);
        }

        [HttpGet("usages")]
        [ForbidIfMissing(Operation.ReadCommercialOffers)]
        public Task<IReadOnlyCollection<CommercialOfferUsage>> GetUsagesAsync([FromQuery] CommercialOfferUsageQuery query)
        {
            return _commercialOffersRepository.GetUsagesAsync(query.ToFilter());
        }

        [Route("upload-csv")]
        [HttpPost, ForbidIfMissing(Operation.CreateCommercialOffers)]
        public async Task<ImportedOffersDto> UploadAsync([FromForm] FileDto file)
        {
            if (file is null || file.File is null)
                throw new ArgumentNullException(nameof(file));

            using var ms = new MemoryStream();

            await file.File.CopyToAsync(ms);
            ms.Position = 0;

            return new ImportedOffersDto { Items = await _importedOfferService.UploadAsync(ms) };
        }

        [Route("upload-csv/template")]
        [HttpPost, ForbidIfMissing(Operation.CreateCommercialOffers)]
        public async Task<FileStreamResult> GetTemplateAsync()
        {
            var ms = await _importedOfferService.GetTemplateStreamAsync();
            return new FileStreamResult(ms, "text/csv")
            {
                FileDownloadName = "offers-template.csv"
            };
        }
    }

    public class ImportedOffersDto
    {
        public IEnumerable<ParsedOffer> Items { get; set; }
    }

    public class CommercialOfferQuery
    {
        public HashSet<int> Id { get; set; } = new HashSet<int>();
        public HashSet<string> Search { get; set; } = new HashSet<string>();
        public HashSet<BillingMode> BillingMode { get; set; } = new HashSet<BillingMode>();
        public HashSet<string> Tag { get; set; } = new HashSet<string>();
        public HashSet<int> ProductId { get; set; } = new HashSet<int>();
        public HashSet<int> CurrencyId { get; set; } = new HashSet<int>();
        public HashSet<bool> IsArchived { get; set; } = new HashSet<bool> { true, false };

        public IPageToken PageToken { get; set; } = null;

        internal CommercialOfferFilter ToFilter()
        {
            return new CommercialOfferFilter
            {
                Ids = Id,
                Search = Search,
                BillingModes = BillingMode,
                Tags = Tag,
                ProductIds = ProductId,
                CurrencyIds = CurrencyId,
                IsArchived = IsArchived.ToCompareBoolean(),
            };
        }
    }

    public class CommercialOfferUsageQuery
    {
        public HashSet<int> OfferId { get; set; } = new HashSet<int>();

        internal CommercialOfferUsageFilter ToFilter()
        {
            return new CommercialOfferUsageFilter
            {
                OfferIds = OfferId
            };
        }
    }

    public class ImportedOffersDto
    {
        public IEnumerable<ParsedOffer> Items { get; set; }
    }

    public class FileDto
    {
        public IFormFile File { get; set; }
    }
}
