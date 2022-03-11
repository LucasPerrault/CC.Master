using System;
using Billing.Contracts.Domain.Counts;
using Billing.Contracts.Domain.Counts.Filtering;
using Billing.Contracts.Domain.Counts.Interfaces;
using Microsoft.EntityFrameworkCore;
using Storage.Infra.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Rights.Domain.Filtering;

namespace Billing.Contracts.Infra.Storage.Stores
{
    public class CountsStore : ICountsStore
    {
        private readonly ContractsDbContext _dbContext;

        public CountsStore(ContractsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyCollection<Count>> GetAsync(AccessRight accessRight, CountFilter filter)
        {
            var queryable = GetQueryable(accessRight, filter).AsNoTracking();
            return await queryable.ToListAsync();
        }

        private IQueryable<Count> GetQueryable(AccessRight accessRight, CountFilter filter)
        {
            return Counts
                .WhereHasRight(accessRight)
                .WhereMatches(filter);
        }

        private IQueryable<Count> Counts => _dbContext.Set<Count>();
    }

    internal static class CountsQueryableExtensions
    {
        public static IQueryable<Count> WhereMatches(this IQueryable<Count> counts, CountFilter filter)
        {
            return counts
                .WhenNotNullOrEmpty(filter.Ids).ApplyWhere(c => filter.Ids.Contains(c.Id))
                .WhenNotNullOrEmpty(filter.CommercialOfferIds).ApplyWhere(c => filter.CommercialOfferIds.Contains(c.CommercialOfferId))
                .WhenNotNullOrEmpty(filter.ContractIds).ApplyWhere(c => filter.ContractIds.Contains(c.ContractId))
                .WhenNotNullOrEmpty(filter.Periods).ApplyWhere(c => filter.Periods.Contains(c.CountPeriod));
        }

        public static IQueryable<Count> WhereHasRight(this IQueryable<Count> counts, AccessRight accessRight)
        {
            return counts.Where(accessRight.ToRightExpression());
        }

        private static Expression<Func<Count, bool>> ToRightExpression(this AccessRight accessRight)
        {
            return accessRight switch
            {
                NoAccessRight _ => _ => false,
                AllAccessRight _ => _ => true,
                _ => throw new ApplicationException($"Unknown type of count filter right {accessRight}")
            };
        }
    }
}
