using Billing.Contracts.Domain.Offers;
using Billing.Contracts.Domain.Offers.Filtering;
using Billing.Contracts.Domain.Offers.Interfaces;
using Lucca.Core.Api.Abstractions.Paging;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Billing.Contracts.Application.Offers
{
    public class CommercialOffersRepository
    {
        private readonly ICommercialOffersStore _commercialOffersStore;
        private readonly ICommercialOfferUsageService _usageService;
        private readonly CommercialOfferRightsFilter _rightsFilter;
        private readonly ClaimsPrincipal _principal;

        public CommercialOffersRepository(ICommercialOffersStore commercialOffersStore, ICommercialOfferUsageService usageService, CommercialOfferRightsFilter rightsFilter, ClaimsPrincipal principal)
        {
            _commercialOffersStore = commercialOffersStore;
            _usageService = usageService;
            _rightsFilter = rightsFilter;
            _principal = principal;
        }

        public async Task<Page<CommercialOffer>> GetPageAsync(IPageToken pageToken, CommercialOfferFilter commercialOfferFilter)
        {
            var accessRight = await _rightsFilter.GetReadAccessAsync(_principal);
            return await _commercialOffersStore.GetPageAsync(accessRight, commercialOfferFilter, pageToken);
        }

        public async Task<CommercialOffer> GetByIdAsync(int id)
        {
            var accessRight = await _rightsFilter.GetReadAccessAsync(_principal);
            var filter = new CommercialOfferFilter { Ids = new HashSet<int> { id } };
            return await _commercialOffersStore.GetSingleOfDefaultAsync(filter, accessRight);
        }

        public async Task<Page<string>> GetTagsAsync()
        {
            var accessRight = await _rightsFilter.GetReadAccessAsync(_principal);
            return await _commercialOffersStore.GetTagsAsync(accessRight);
        }

        public async Task<CommercialOffer> CreateAsync(CommercialOffer offer)
        {
            var accessRight = await _rightsFilter.GetWriteAccessAsync(_principal);
            return await _commercialOffersStore.CreateAsync(offer, accessRight);
        }

        public async Task<IReadOnlyCollection<CommercialOffer>> CreateManyAsync(IReadOnlyCollection<CommercialOffer> offers)
        {
            var accessRight = await _rightsFilter.GetWriteAccessAsync(_principal);
            return await _commercialOffersStore.CreateManyAsync(offers, accessRight);
        }

        public async Task PutAsync(int id, CommercialOffer offer)
        {
            var accessRight = await _rightsFilter.GetWriteAccessAsync(_principal);
            await _commercialOffersStore.PutAsync(id, offer, accessRight);
        }

        public async Task DeleteAsync(int id)
        {
            var accessRight = await _rightsFilter.GetWriteAccessAsync(_principal);
            await _commercialOffersStore.ArchiveAsync(id, accessRight);
        }

        public async Task<CommercialOffer> AddPriceListAsync(int id, PriceList priceList)
        {
            var accessRight = await _rightsFilter.GetWriteAccessAsync(_principal);
            return await _commercialOffersStore.AddPriceListAsync(id, priceList, accessRight);
        }

        public async Task ModifyPriceListAsync(int id, int listId, PriceList priceList)
        {
            var accessRight = await _rightsFilter.GetWriteAccessAsync(_principal);
            await _commercialOffersStore.ModifyPriceListAsync(id, listId, priceList, accessRight);
        }

        public async Task DeletePriceListAsync(int id, int listId)
        {
            var accessRight = await _rightsFilter.GetWriteAccessAsync(_principal);
            await _commercialOffersStore.DeletePriceListAsync(id, listId, accessRight);
        }

        public async Task<IReadOnlyCollection<CommercialOfferUsage>> GetUsagesAsync(CommercialOfferUsageFilter filter)
        {
            var accessRight = await _rightsFilter.GetReadAccessAsync(_principal);
            return await _usageService.BuildAsync(filter.OfferIds, accessRight);
        }
    }
}
