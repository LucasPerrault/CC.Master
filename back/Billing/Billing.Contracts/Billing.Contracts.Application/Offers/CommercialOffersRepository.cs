using Billing.Contracts.Domain.Offers;
using Billing.Contracts.Domain.Offers.Filtering;
using Billing.Contracts.Domain.Offers.Interfaces;
using Billing.Contracts.Domain.Offers.Validation;
using Lucca.Core.Api.Abstractions.Paging;
using Rights.Domain.Filtering;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Billing.Contracts.Application.Offers
{
    public class CommercialOffersRepository
    {
        private readonly ICommercialOffersStore _store;
        private readonly ICommercialOfferUsageService _usageService;
        private readonly CommercialOfferValidationService _validation;
        private readonly CommercialOfferRightsFilter _rightsFilter;
        private readonly ClaimsPrincipal _principal;

        public CommercialOffersRepository
        (
            ICommercialOffersStore commercialOffersStore,
            ICommercialOfferUsageService usageService,
            CommercialOfferValidationService validation,
            CommercialOfferRightsFilter rightsFilter,
            ClaimsPrincipal principal
        )
        {
            _store = commercialOffersStore;
            _usageService = usageService;
            _validation = validation;
            _rightsFilter = rightsFilter;
            _principal = principal;
        }

        public async Task<Page<CommercialOffer>> GetPageAsync(IPageToken pageToken, CommercialOfferFilter commercialOfferFilter)
        {
            var accessRight = await _rightsFilter.GetReadAccessAsync(_principal);
            return await _store.GetPageAsync(accessRight, commercialOfferFilter, pageToken);
        }

        public async Task<CommercialOffer> GetByIdAsync(int id)
        {
            var accessRight = await _rightsFilter.GetReadAccessAsync(_principal);
            return await _store.GetReadOnlySingleOfDefaultAsync(CommercialOfferFilter.ForId(id), accessRight);
        }

        public async Task<Page<string>> GetTagsAsync()
        {
            var accessRight = await _rightsFilter.GetReadAccessAsync(_principal);
            return await _store.GetTagsAsync(accessRight);
        }

        public async Task<CommercialOffer> CreateAsync(CommercialOffer offer)
        {
            var accessRight = await _rightsFilter.GetWriteAccessAsync(_principal);
            _validation.ThrowIfCannotCreateOffer(offer);

            return await _store.CreateAsync(offer, accessRight);
        }

        public async Task<IReadOnlyCollection<CommercialOffer>> CreateManyAsync(IReadOnlyCollection<CommercialOffer> offers)
        {
            var accessRight = await _rightsFilter.GetWriteAccessAsync(_principal);
            foreach (var offer in offers)
            {
                _validation.ThrowIfCannotCreateOffer(offer);
            }

            return await _store.CreateManyAsync(offers, accessRight);
        }

        public async Task PutAsync(int id, CommercialOffer offer)
        {
            var accessRight = await _rightsFilter.GetWriteAccessAsync(_principal);

            var oldOffer = await GetReadOnlyByIdWithoutRightAsync(id);
            var usage = await GetOfferUsageAsync(id);
            _validation.ThrowIfCannotModifyOffer(oldOffer, offer, usage);

            await _store.PutAsync(id, offer, accessRight);
        }

        public async Task DeleteAsync(int id)
        {
            var accessRight = await _rightsFilter.GetWriteAccessAsync(_principal);

            var offer = await GetReadOnlyByIdWithoutRightAsync(id);
            var usage = await GetOfferUsageAsync(id);
            _validation.ThrowIfCannotDeleteOffer(offer, usage);

            await _store.ArchiveAsync(id, accessRight);
        }

        public async Task<CommercialOffer> AddPriceListAsync(int id, PriceList priceList)
        {
            var accessRight = await _rightsFilter.GetWriteAccessAsync(_principal);

            var offer = await GetReadOnlyByIdWithoutRightAsync(id);
            _validation.ThrowIfCannotAddPriceList(offer, priceList);

            return await _store.AddPriceListAsync(id, priceList, accessRight);
        }

        public async Task ModifyPriceListAsync(int id, int listId, PriceList priceList)
        {
            var accessRight = await _rightsFilter.GetWriteAccessAsync(_principal);

            var offer = await GetReadOnlyByIdWithoutRightAsync(id);
            var oldPriceList = offer.PriceLists.Single(pl => pl.Id == listId);
            var usage = await GetOfferUsageAsync(id);
            _validation.ThrowIfCannotModifyPriceList(offer, oldPriceList, priceList, usage);

            await _store.ModifyPriceListAsync(id, listId, priceList, accessRight);
        }

        public async Task DeletePriceListAsync(int id, int listId)
        {
            var accessRight = await _rightsFilter.GetWriteAccessAsync(_principal);

            var offer = await GetReadOnlyByIdWithoutRightAsync(id);
            var priceList = offer.PriceLists.Single(pl => pl.Id == listId);
            _validation.ThrowIfCannotDeletePriceList(offer, priceList);

            await _store.DeletePriceListAsync(id, listId, accessRight);
        }

        public async Task<IReadOnlyCollection<CommercialOfferUsage>> GetUsagesAsync(CommercialOfferUsageFilter filter)
        {
            var accessRight = await _rightsFilter.GetReadAccessAsync(_principal);
            return await _usageService.BuildAsync(filter.OfferIds, accessRight);
        }

        private Task<CommercialOffer> GetReadOnlyByIdWithoutRightAsync(int id)
        {
            return _store.GetReadOnlySingleOfDefaultAsync(CommercialOfferFilter.ForId(id), AccessRight.All);
        }

        private Task<CommercialOfferUsage> GetOfferUsageAsync(int id)
        {
            return _usageService.BuildAsync(id);
        }
    }
}
