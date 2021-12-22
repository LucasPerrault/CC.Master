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
    public class ContractsStore : IContractsStore
    {
        private readonly AdvancedFiltersDbContext _dbContext;
        private readonly IQueryPager _queryPager;

        public ContractsStore(AdvancedFiltersDbContext dbContext, IQueryPager queryPager)
        {
            _dbContext = dbContext;
            _queryPager = queryPager;
        }

        public Task<Page<Contract>> GetAsync(IPageToken pageToken, ContractFilter filter)
        {
            var contracts = Get(filter);
            return _queryPager.ToPageAsync(contracts, pageToken);
        }

        private IQueryable<Contract> Get(ContractFilter filter)
        {
            return Contracts
                .WhereMatches(filter)
                .AsNoTracking();
        }

        private IQueryable<Contract> Contracts => _dbContext
            .Set<Contract>()
            .Include(c => c.Client);
    }

    internal static class ContractQueryableExtensions
    {
        public static IQueryable<Contract> WhereMatches(this IQueryable<Contract> contracts, ContractFilter filter)
        {
            return contracts;
        }
    }
}
