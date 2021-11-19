using Billing.Contracts.Domain.Counts;
using Billing.Contracts.Domain.Counts.Filtering;
using Billing.Contracts.Domain.Counts.Interfaces;
using Microsoft.EntityFrameworkCore;
using Storage.Infra.Extensions;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<IReadOnlyCollection<Count>> GetAsync(CountFilter filter)
        {
            return await Counts
                .WhereMatches(filter)
                .ToListAsync();
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
                .WhenNotNullOrEmpty(filter.ContractIds).ApplyWhere(c => filter.ContractIds.Contains(c.ContractId));
        }
    }
}
