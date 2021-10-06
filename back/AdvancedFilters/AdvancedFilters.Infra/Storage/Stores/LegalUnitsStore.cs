using AdvancedFilters.Domain.Core.Collections;
using AdvancedFilters.Domain.Core.Models;
using AdvancedFilters.Domain.Instance.Filters;
using AdvancedFilters.Domain.Instance.Interfaces;
using AdvancedFilters.Domain.Instance.Models;
using Lucca.Core.Api.Abstractions.Paging;
using Lucca.Core.Api.Queryable.Paging;
using Microsoft.EntityFrameworkCore;
using Storage.Infra.Extensions;
using System.Linq;
using System.Threading.Tasks;

namespace AdvancedFilters.Infra.Storage.Stores
{
    public class LegalUnitsStore : ILegalUnitsStore
    {
        private readonly AdvancedFiltersDbContext _dbContext;
        private readonly IQueryPager _queryPager;
        private readonly ICountriesCollection _countriesCollection;

        public LegalUnitsStore
        (
            AdvancedFiltersDbContext dbContext,
            IQueryPager queryPager,
            ICountriesCollection countriesCollection
        )
        {
            _dbContext = dbContext;
            _queryPager = queryPager;
            _countriesCollection = countriesCollection;
        }

        public Task<Page<LegalUnit>> GetAsync(IPageToken pageToken, LegalUnitFilter filter)
        {
            var lus = Get(filter);
            return _queryPager.ToPageAsync(lus, pageToken);
        }

        public async Task<Page<Country>> GetAllCountriesAsync()
        {
            var luCountryIds = LegalUnits
                .AsNoTracking()
                .Select(lu => lu.CountryId)
                .Distinct()
                .ToList();
            var luCountries = await _countriesCollection.GetAsync(luCountryIds);

            return new Page<Country>
            {
                Count = luCountries.Count,
                Items = luCountries
            };
        }

        private IQueryable<LegalUnit> Get(LegalUnitFilter filter)
        {
            return LegalUnits
                .WhereMatches(filter)
                .AsNoTracking();
        }

        private IQueryable<LegalUnit> LegalUnits => _dbContext
            .Set<LegalUnit>()
            .Include(lu => lu.Environment).ThenInclude(e => e.AppInstances)
            .Include(lu => lu.Establishments).ThenInclude(e => e.Environment);
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
