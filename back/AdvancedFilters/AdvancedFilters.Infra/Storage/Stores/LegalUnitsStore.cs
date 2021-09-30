using AdvancedFilters.Domain.Core.Collections;
using AdvancedFilters.Domain.Core.Models;
using AdvancedFilters.Domain.Instance.Filters;
using AdvancedFilters.Domain.Instance.Interfaces;
using AdvancedFilters.Domain.Instance.Models;
using Lucca.Core.Api.Abstractions.Paging;
using Lucca.Core.Api.Queryable.Paging;
using Microsoft.EntityFrameworkCore;
using Storage.Infra.Extensions;
using System.Collections.Generic;
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

        public async Task<Page<LegalUnit>> GetAsync(IPageToken pageToken, LegalUnitFilter filter)
        {
            var lus = Get(filter);
            var page = await _queryPager.ToPageAsync(lus, pageToken);

            var items = new List<LegalUnit>();
            foreach (var lu in page.Items)
            {
                items.Add(await PopulateAsync(lu));
            }

            return new Page<LegalUnit>
            {
                Count = page.Count,
                Items = items,
                Next = page.Next,
                Prev = page.Prev
            };
        }

        public async Task<Page<Country>> GetAllCountriesAsync()
        {
            var luCountryIds = LegalUnits
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
                .WhereMatches(filter);
        }

        private async Task<LegalUnit> PopulateAsync(LegalUnit lu)
        {
            lu.Country = await _countriesCollection.GetByIdAsync(lu.CountryId);

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
