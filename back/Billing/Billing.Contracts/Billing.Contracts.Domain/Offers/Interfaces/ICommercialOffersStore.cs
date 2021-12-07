using Billing.Contracts.Domain.Offers.Filtering;
using Lucca.Core.Api.Abstractions.Paging;
using Rights.Domain.Filtering;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Billing.Contracts.Domain.Offers.Interfaces
{
    public interface ICommercialOffersStore
    {
        Task<Page<CommercialOffer>> GetPageAsync(AccessRight accessRight, CommercialOfferFilter filter, IPageToken pageToken);
        Task<Page<CommercialOffer>> GetSimilarOffersAsync(AccessRight accessRight, CommercialOffer referenceOffer, DateTime until);
        Task<CommercialOffer> GetSingleOrDefaultAsync(CommercialOfferFilter filter, AccessRight accessRight);
        Task<CommercialOffer> GetReadOnlySingleOrDefaultAsync(CommercialOfferFilter filter, AccessRight accessRight);
        Task<Page<string>> GetTagsAsync(CommercialOfferTagFilter filter, AccessRight accessRight);
        Task<CommercialOffer> CreateAsync(CommercialOffer offer, AccessRight accessRight);
        Task<IReadOnlyCollection<CommercialOffer>> CreateManyAsync(IReadOnlyCollection<CommercialOffer> offers, AccessRight accessRight);
        Task PutAsync(int id, CommercialOffer offer, AccessRight accessRight);
        Task<CommercialOffer> AddPriceListAsync(int id, PriceList priceList, AccessRight accessRight);
        Task ModifyPriceListAsync(int id, int listId, PriceList priceList, AccessRight accessRight);
        Task DeletePriceListAsync(int id, int listId, AccessRight accessRight);
    }
}
