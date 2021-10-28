using AdvancedFilters.Domain.Billing.Filters;
using AdvancedFilters.Domain.Billing.Interfaces;
using AdvancedFilters.Domain.Billing.Models;
using Lucca.Core.Api.Abstractions.Paging;
using Lucca.Core.Api.Queryable.Paging;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace AdvancedFilters.Infra.Storage.Stores
{
    public class DistributorsStore : IDistributorsStore
    {
        private readonly AdvancedFiltersDbContext _dbContext;
        private readonly IQueryPager _queryPager;

        public DistributorsStore(AdvancedFiltersDbContext dbContext, IQueryPager queryPager)
        {
            _dbContext = dbContext;
            _queryPager = queryPager;
        }

        public Task<Page<Distributor>> GetAsync(IPageToken pageToken, DistributorFilter filter)
        {
            var clients = Get(filter);
            return _queryPager.ToPageAsync(clients, pageToken);
        }

        private IQueryable<Distributor> Get(DistributorFilter filter)
        {
            return Distributors
                .WhereMatches(filter)
                .AsNoTracking();
        }

        private IQueryable<Distributor> Distributors => _dbContext
            .Set<Distributor>();
    }

    internal static class DistributorQueryableExtensions
    {
        public static IQueryable<Distributor> WhereMatches(this IQueryable<Distributor> distributors, DistributorFilter filter)
        {
            return distributors;
        }
    }
}
