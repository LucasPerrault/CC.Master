using AdvancedFilters.Domain.Billing.Filters;
using AdvancedFilters.Domain.Billing.Interfaces;
using AdvancedFilters.Domain.Billing.Models;
using Lucca.Core.Api.Abstractions.Paging;
using Lucca.Core.Api.Queryable.Paging;
using Microsoft.EntityFrameworkCore;
using Storage.Infra.Extensions;
using System.Linq;
using System.Threading.Tasks;

namespace AdvancedFilters.Infra.Storage.Stores
{
    public class ClientsStore : IClientsStore
    {
        private readonly AdvancedFiltersDbContext _dbContext;
        private readonly IQueryPager _queryPager;

        public ClientsStore(AdvancedFiltersDbContext dbContext, IQueryPager queryPager)
        {
            _dbContext = dbContext;
            _queryPager = queryPager;
        }

        public Task<Page<Client>> GetAsync(IPageToken pageToken, ClientFilter filter)
        {
            var clients = Get(filter);
            return _queryPager.ToPageAsync(clients, pageToken);
        }

        private IQueryable<Client> Get(ClientFilter filter)
        {
            return Clients
                .WhereMatches(filter)
                .AsNoTracking();
        }

        private IQueryable<Client> Clients => _dbContext
            .Set<Client>()
            .Include(c => c.Contracts);
    }

    internal static class ClientQueryableExtensions
    {
        public static IQueryable<Client> WhereMatches(this IQueryable<Client> clients, ClientFilter filter)
        {
            return clients
                .WhenNotNullOrEmpty(filter.Search).ApplyWhere(c => c.Name.Contains(filter.Search));
        }
    }
}
