using Billing.Contracts.Domain.Counts;
using Billing.Contracts.Domain.Counts.Filtering;
using Billing.Contracts.Domain.Counts.Interfaces;
using Microsoft.EntityFrameworkCore;
using Rights.Domain.Filtering;
using Storage.Infra.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

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
            return await Counts
                .WhereHasAccess(accessRight)
                .AsNoTracking()
                .WhereMatches(filter)
                .ToListAsync();
        }

        private IQueryable<Count> Counts => _dbContext.Set<Count>();
    }

    internal static class CountsQueryableExtensions
    {
        public static IQueryable<Count> WhereHasAccess(this IQueryable<Count> counts, AccessRight accessRight)
        {
            return counts.Where(AccessExpression(accessRight));
        }

        private static Expression<Func<Count, bool>> AccessExpression(AccessRight accessRight) => accessRight switch
        {
            AllAccessRight _ => _ => true,
            NoAccessRight _ => _ => false,
            DistributorAccessRight r => c => c.Contract.DistributorId == r.DistributorId,
        };

        public static IQueryable<Count> WhereMatches(this IQueryable<Count> counts, CountFilter filter)
        {
            return counts
                .WhenNotNullOrEmpty(filter.Ids).ApplyWhere(c => filter.Ids.Contains(c.Id))
                .WhenNotNullOrEmpty(filter.CommercialOfferIds).ApplyWhere(c => filter.CommercialOfferIds.Contains(c.CommercialOfferId))
                .WhenNotNullOrEmpty(filter.ContractIds).ApplyWhere(c => filter.ContractIds.Contains(c.ContractId));
        }
    }
}
