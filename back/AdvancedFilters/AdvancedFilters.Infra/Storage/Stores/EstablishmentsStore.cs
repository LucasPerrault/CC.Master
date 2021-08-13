using AdvancedFilters.Domain.Billing.Filters;
using AdvancedFilters.Domain.Billing.Interfaces;
using AdvancedFilters.Domain.Billing.Models;
using Lucca.Core.Api.Abstractions.Paging;
using Lucca.Core.Api.Queryable.Paging;
using System.Linq;
using System.Threading.Tasks;

namespace AdvancedFilters.Infra.Storage.Stores
{
    public class EstablishmentsStore : IEstablishmentsStore
    {
        private readonly AdvancedFiltersDbContext _dbContext;
        private readonly IQueryPager _queryPager;

        public EstablishmentsStore(AdvancedFiltersDbContext dbContext, IQueryPager queryPager)
        {
            _dbContext = dbContext;
            _queryPager = queryPager;
        }

        public Task<Page<Establishment>> GetAsync(IPageToken pageToken, EstablishmentFilter filter)
        {
            var establishments = Get(filter);
            return _queryPager.ToPageAsync(establishments, pageToken);
        }

        private IQueryable<Establishment> Get(EstablishmentFilter filter)
        {
            return Establishments
                .WhereMatches(filter);
        }

        private IQueryable<Establishment> Establishments => _dbContext.Set<Establishment>();
    }

    internal static class EstablishmentQueryableExtensions
    {
        public static IQueryable<Establishment> WhereMatches(this IQueryable<Establishment> establishments, EstablishmentFilter filter)
        {
            return establishments;
        }
    }
}
