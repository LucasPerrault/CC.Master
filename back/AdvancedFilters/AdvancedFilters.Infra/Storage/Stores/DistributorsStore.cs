using AdvancedFilters.Domain.Billing.Filters;
using AdvancedFilters.Domain.Billing.Interfaces;
using AdvancedFilters.Domain.Billing.Models;
using Lucca.Core.Api.Abstractions.Paging;
using Microsoft.EntityFrameworkCore;
using Storage.Infra.Extensions;
using System.Linq;
using System.Threading.Tasks;

namespace AdvancedFilters.Infra.Storage.Stores
{
    public class DistributorsStore : IDistributorsStore
    {
        private readonly AdvancedFiltersDbContext _dbContext;

        public DistributorsStore(AdvancedFiltersDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<Page<Distributor>> GetAsync(DistributorFilter filter)
        {
            var distributors = Get(filter).ToList();

            return Task.FromResult(new Page<Distributor>
            {
                Count = distributors.Count,
                Items = distributors
            });
        }

        private IQueryable<Distributor> Get(DistributorFilter filter)
        {
            return Distributors
                .WhereMatches(filter)
                .AsNoTracking();
        }

        private IQueryable<Distributor> Distributors => _dbContext.Set<Distributor>();
    }

    internal static class DistributorQueryableExtensions
    {
        public static IQueryable<Distributor> WhereMatches(this IQueryable<Distributor> distributors, DistributorFilter filter)
        {
            return distributors
                .WhenNotNullOrEmpty(filter.Search).ApplyWhere(c => c.Name.Contains(filter.Search));
        }
    }
}
