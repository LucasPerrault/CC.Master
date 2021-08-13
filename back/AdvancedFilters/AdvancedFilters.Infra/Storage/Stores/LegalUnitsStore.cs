using AdvancedFilters.Domain.Billing.Filters;
using AdvancedFilters.Domain.Billing.Interfaces;
using AdvancedFilters.Domain.Billing.Models;
using Lucca.Core.Api.Abstractions.Paging;
using Lucca.Core.Api.Queryable.Paging;
using Storage.Infra.Extensions;
using System.Linq;
using System.Threading.Tasks;

namespace AdvancedFilters.Infra.Storage.Stores
{
    public class LegalUnitsStore : ILegalUnitsStore
    {
        private readonly AdvancedFiltersDbContext _dbContext;
        private readonly IQueryPager _queryPager;

        public LegalUnitsStore(AdvancedFiltersDbContext dbContext, IQueryPager queryPager)
        {
            _dbContext = dbContext;
            _queryPager = queryPager;
        }

        public Task<Page<LegalUnit>> GetAsync(IPageToken pageToken, LegalUnitFilter filter)
        {
            var lus = Get(filter);
            return _queryPager.ToPageAsync(lus, pageToken);
        }

        private IQueryable<LegalUnit> Get(LegalUnitFilter filter)
        {
            return LegalUnits
                .WhereMatches(filter);
        }

        private IQueryable<LegalUnit> LegalUnits => _dbContext.Set<LegalUnit>();
    }

    internal static class LegalUnitQueryableExtensions
    {
        public static IQueryable<LegalUnit> WhereMatches(this IQueryable<LegalUnit> lus, LegalUnitFilter filter)
        {
            return lus
                .WhenNotNullOrEmpty(filter.CountryIds).ApplyWhere(lu => filter.CountryIds.Contains(lu.CountryId))
                .WhenNotNullOrEmpty(filter.EnvironmentIds).ApplyWhere(lu => filter.EnvironmentIds.Contains(lu.EnvironmentId));
        }
    }
}
