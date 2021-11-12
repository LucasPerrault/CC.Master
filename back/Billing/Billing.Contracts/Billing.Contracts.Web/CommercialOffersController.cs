using Billing.Contracts.Application.Offers;
using Billing.Contracts.Domain.Offers;
using Billing.Contracts.Domain.Offers.Filtering;
using Lucca.Core.Api.Abstractions.Paging;
using Lucca.Core.Api.Web.ModelBinding.Sorting;
using Microsoft.AspNetCore.Mvc;
using Rights.Domain;
using Rights.Web.Attributes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Billing.Contracts.Web
{
    [ApiController, Route("/api/commercial-offers")]
    [ApiSort(nameof(CommercialOffer.Id))]
    public class CommercialOffersController
    {
        private readonly CommercialOffersRepository _commercialOffersRepository;

        public CommercialOffersController(CommercialOffersRepository commercialOffersRepository)
        {
            _commercialOffersRepository = commercialOffersRepository;
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
    }

    public class CommercialOfferQuery
    {
        public IPageToken PageToken { get; set; }

        internal CommercialOfferFilter ToFilter()
        {
            return new CommercialOfferFilter
            {

            };
        }
    }
}
