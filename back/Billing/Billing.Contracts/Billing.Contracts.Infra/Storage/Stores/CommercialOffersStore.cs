using Billing.Contracts.Domain.Offers;
using Billing.Contracts.Domain.Offers.Filtering;
using Billing.Contracts.Domain.Offers.Interfaces;
using Lucca.Core.Api.Abstractions.Paging;
using Lucca.Core.Api.Queryable.Paging;
using Microsoft.EntityFrameworkCore;
using Rights.Domain.Filtering;
using Storage.Infra.Extensions;
using Storage.Infra.Querying;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Billing.Contracts.Infra.Storage.Stores
{
    public class CommercialOffersStore : ICommercialOffersStore
    {
        private readonly ContractsDbContext _dbContext;
        private readonly IQueryPager _queryPager;

        public CommercialOffersStore
        (
            ContractsDbContext dbContext,
            IQueryPager queryPager
        )
        {
            _dbContext = dbContext;
            _queryPager = queryPager;
        }

        public Task<Page<CommercialOffer>> GetPageAsync(AccessRight accessRight, CommercialOfferFilter filter, IPageToken pageToken)
        {
            var queryable = GetQueryable(accessRight, filter).AsNoTracking();
            return _queryPager.ToPageAsync(queryable, pageToken);
        }

        public async Task<Page<CommercialOffer>> GetSimilarOffersAsync(AccessRight accessRight, CommercialOffer referenceOffer, DateTime until)
        {
            var filter = CommercialOfferFilter.SimilarTo(referenceOffer);
            var similarOffers = await GetQueryable(accessRight, filter)
                .AsNoTracking()
                .ToListAsync();

            var rowsFilteredSimilarOffers = similarOffers
                .Where(o => o.IsSimilarUntil(referenceOffer, until))
                .ToList();

            return new Page<CommercialOffer>
            {
                Items = rowsFilteredSimilarOffers,
                Count = rowsFilteredSimilarOffers.Count,
            };
        }

        public Task<CommercialOffer> GetSingleOrDefaultAsync(CommercialOfferFilter filter, AccessRight accessRight)
        {
            return GetQueryable(accessRight, filter).SingleOrDefaultAsync();
        }

        public Task<CommercialOffer> GetReadOnlySingleOrDefaultAsync(CommercialOfferFilter filter, AccessRight accessRight)
        {
            return GetQueryable(accessRight, filter).AsNoTracking().SingleOrDefaultAsync();
        }

        public Task<Page<string>> GetTagsAsync(AccessRight accessRight)
        {
            var tags = GetQueryable(accessRight, CommercialOfferFilter.All)
                .AsNoTracking()
                .Select(o => o.Tag)
                .Distinct()
                .ToList();

            return Task.FromResult(new Page<string>
            {
                Count = tags.Count,
                Items = tags
            });
        }

        public async Task<CommercialOffer> CreateAsync(CommercialOffer offer, AccessRight accessRight)
        {
            _dbContext.Add(offer);
            await _dbContext.SaveChangesAsync();

            return offer;
        }

        public async Task<IReadOnlyCollection<CommercialOffer>> CreateManyAsync(IReadOnlyCollection<CommercialOffer> offers, AccessRight accessRight)
        {
            _dbContext.AddRange(offers);
            await _dbContext.SaveChangesAsync();

            return offers;
        }

        public async Task PutAsync(int id, CommercialOffer offer, AccessRight accessRight)
        {
            _dbContext.Update(offer);
            await _dbContext.SaveChangesAsync();
        }

        public async Task ArchiveAsync(int id, AccessRight accessRight)
        {
            var offer = await GetSingleOrDefaultAsync(CommercialOfferFilter.ForId(id), accessRight);

            offer.IsArchived = true;

            await _dbContext.SaveChangesAsync();
        }

        public async Task<CommercialOffer> AddPriceListAsync(int id, PriceList priceList, AccessRight accessRight)
        {
            var offer = await GetSingleOrDefaultAsync(CommercialOfferFilter.ForId(id), accessRight);

            offer.PriceLists.Add(priceList);

            await _dbContext.SaveChangesAsync();

            return offer;
        }

        public async Task ModifyPriceListAsync(int id, int listId, PriceList priceList, AccessRight accessRight)
        {
            var offer = await GetSingleOrDefaultAsync(CommercialOfferFilter.ForId(id), accessRight);
            var oldPriceList = offer.PriceLists.Single(pl => pl.Id == listId);

            offer.PriceLists.Remove(oldPriceList);
            offer.PriceLists.Add(priceList);

            await _dbContext.SaveChangesAsync();
        }

        public async Task DeletePriceListAsync(int id, int listId, AccessRight accessRight)
        {
            var offer = await GetSingleOrDefaultAsync(CommercialOfferFilter.ForId(id), accessRight);
            var priceList = offer.PriceLists.Single(pl => pl.Id == listId);

            offer.PriceLists.Remove(priceList);

            await _dbContext.SaveChangesAsync();
        }

        private IQueryable<CommercialOffer> GetQueryable(AccessRight accessRight, CommercialOfferFilter filter)
        {
            return Offers
                .WhereHasRight(accessRight)
                .WhereMatches(filter);
        }

        private IQueryable<CommercialOffer> Offers => _dbContext.Set<CommercialOffer>()
            .Include(o => o.Product)
            .Include(o => o.PriceLists).ThenInclude(pl => pl.Rows);
    }

    internal static class CommercialOffersQueryableExtensions
    {
        public static IQueryable<CommercialOffer> WhereMatches(this IQueryable<CommercialOffer> offers, CommercialOfferFilter filter)
        {
            return offers
                .Search(filter.Search)
                .WhenNotNullOrEmpty(filter.Ids).ApplyWhere(o => filter.Ids.Contains(o.Id))
                .WhenNotNullOrEmpty(filter.BillingModes).ApplyWhere(o => filter.BillingModes.Contains(o.BillingMode))
                .WhenNotNullOrEmpty(filter.Tags).ApplyWhere(o => filter.Tags.Contains(o.Tag))
                .WhenNotNullOrEmpty(filter.ProductIds).ApplyWhere(o => filter.ProductIds.Contains(o.ProductId))
                .WhenNotNullOrEmpty(filter.CurrencyIds).ApplyWhere(o => filter.CurrencyIds.Contains(o.CurrencyId))
                .Apply(filter.IsArchived).To(o => o.IsArchived);
        }

        public static IQueryable<CommercialOffer> WhereHasRight(this IQueryable<CommercialOffer> offers, AccessRight accessRight)
        {
            return offers.Where(accessRight.ToRightExpression());
        }

        private static IQueryable<CommercialOffer> Search(this IQueryable<CommercialOffer> offers, HashSet<string> words)
        {
            var usableWords = words.Sanitize();
            if (!usableWords.Any())
            {
                return offers;
            }

            Expression<Func<CommercialOffer, bool>> fullTextExpression = o =>
                EF.Functions.Contains(o.Name, usableWords.ToFullTextContainsPredicate());

            return offers.Where(fullTextExpression);
        }

        private static Expression<Func<CommercialOffer, bool>> ToRightExpression(this AccessRight accessRight)
        {
            return accessRight switch
            {
                NoAccessRight _ => _ => false,
                AllAccessRight _ => _ => true,
                _ => throw new ApplicationException($"Unknown type of offer filter right {accessRight}")
            };
        }
    }
}
