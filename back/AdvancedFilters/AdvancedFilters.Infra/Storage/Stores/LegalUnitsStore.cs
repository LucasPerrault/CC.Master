using AdvancedFilters.Domain.Instance.Filters;
using AdvancedFilters.Domain.Instance.Interfaces;
using AdvancedFilters.Domain.Instance.Models;
using Lucca.Core.Api.Abstractions.Paging;
using Lucca.Core.Api.Queryable.Paging;
using Lucca.Core.PublicData.Domain;
using Storage.Infra.Extensions;
using System.Linq;
using System.Threading.Tasks;

namespace AdvancedFilters.Infra.Storage.Stores
{
    public class LegalUnitsStore : ILegalUnitsStore
    {
        private readonly AdvancedFiltersDbContext _dbContext;
        private readonly IQueryPager _queryPager;
        private readonly ILuccaCountriesCollection _countriesCollection;

        public LegalUnitsStore
        (
            AdvancedFiltersDbContext dbContext,
            IQueryPager queryPager,
            ILuccaCountriesCollection countriesCollection
        )
        {
            _dbContext = dbContext;
            _queryPager = queryPager;
            _countriesCollection = countriesCollection;
        }

        public async Task<Page<LegalUnit>> GetAsync(IPageToken pageToken, LegalUnitFilter filter)
        {
            var lus = Get(filter);
            var page = await _queryPager.ToPageAsync(lus, pageToken);

            return new Page<LegalUnit>
            {
                Count = page.Count,
                Items = page.Items.Select(Populate),
                Next = page.Next,
                Prev = page.Prev
            };
        }

        private IQueryable<LegalUnit> Get(LegalUnitFilter filter)
        {
            return LegalUnits
                .WhereMatches(filter)
                .AsQueryable();
        }

        private LegalUnit Populate(LegalUnit lu)
        {
            var luccaCountry = _countriesCollection.GetById(lu.CountryId);
            lu.Country = new Country(luccaCountry);

            return lu;
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
