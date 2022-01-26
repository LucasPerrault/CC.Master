using AdvancedFilters.Domain.Facets;
using AdvancedFilters.Infra.Storage.DAO;
using Lucca.Core.Api.Abstractions.Paging;
using Lucca.Core.Api.Queryable.Paging;
using Microsoft.EntityFrameworkCore;
using Storage.Infra.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdvancedFilters.Infra.Storage.Stores
{
    public class FacetsStore : IFacetsStore
    {
        private readonly AdvancedFiltersDbContext _dbContext;
        private readonly IQueryPager _queryPager;

        public FacetsStore(AdvancedFiltersDbContext dbContext, IQueryPager queryPager)
        {
            _dbContext = dbContext;
            _queryPager = queryPager;
        }

        public Task<Page<Facet>> GetAsync(IPageToken pageToken, FacetScope scope, FacetFilter filter)
        {
            var queryable = Facets(scope).WhereMatches(filter);
            return _queryPager.ToPageAsync(queryable, pageToken);
        }

        public async Task<Page<IEnvironmentFacetValue>> GetValuesAsync(IPageToken pageToken, EnvironmentFacetValueFilter filter)
        {
            var queryable = EnvironmentValues.WhereMatches(filter);
            var page = await _queryPager.ToPageAsync(queryable, pageToken);

            return new Page<IEnvironmentFacetValue>
            {
                Count = page.Count,
                Items = page.Items.ToValues()
            };
        }

        public async Task<List<IEnvironmentFacetValue>> GetValuesAsync(EnvironmentFacetValueFilter filter)
        {
            return (await EnvironmentValues
                .WhereMatches(filter)
                .ToListAsync())
                .ToValues()
                .ToList();
        }

        private IQueryable<Facet> Facets(FacetScope scope) => _dbContext
            .Set<Facet>()
            .Where(f => f.Scope == scope);

        private IQueryable<EnvironmentFacetValueDao> EnvironmentValues => _dbContext
            .Set<EnvironmentFacetValueDao>()
            .Where(v => v.Facet.Type != FacetType.Unknown)
            .Include(v => v.Facet);
    }

    internal static class FacetsExtensions
    {
        public static IQueryable<Facet> WhereMatches(this IQueryable<Facet> facets, FacetFilter filter)
        {
            return facets
                .WhenNotNullOrEmpty(filter.Codes).ApplyWhere(facet => filter.Codes.Contains(facet.Code))
                .WhenNotNullOrEmpty(filter.ApplicationIds).ApplyWhere(facet => filter.ApplicationIds.Contains(facet.ApplicationId))
                .WhenNotNullOrEmpty(filter.FacetTypes).ApplyWhere(facet => filter.FacetTypes.Contains(facet.Type));
        }

        public static IQueryable<EnvironmentFacetValueDao> WhereMatches(this IQueryable<EnvironmentFacetValueDao> daos, EnvironmentFacetValueFilter filter)
        {
            return daos
                .WhenNotNullOrEmpty(filter.Codes).ApplyWhere(dao => filter.Codes.Contains(dao.Facet.Code))
                .WhenNotNullOrEmpty(filter.ApplicationIds).ApplyWhere(dao => filter.ApplicationIds.Contains(dao.Facet.ApplicationId))
                .WhenNotNullOrEmpty(filter.FacetTypes).ApplyWhere(dao => filter.FacetTypes.Contains(dao.Facet.Type))
                .WhenNotNullOrEmpty(filter.EnvironmentIds).ApplyWhere(dao => filter.EnvironmentIds.Contains(dao.EnvironmentId))
                .WhenNotNullOrEmpty(filter.FacetIdentifiers).ApplyWhere(dao => filter.FacetIdentifiers.Any(id => dao.Facet.ApplicationId == id.ApplicationId && dao.Facet.Code == id.Code));
        }

        public static IEnumerable<IEnvironmentFacetValue> ToValues(this IEnumerable<EnvironmentFacetValueDao> daos)
        {
            return daos.Select(dao => dao.ToValue());
        }

        private static IEnvironmentFacetValue ToValue(this EnvironmentFacetValueDao dao)
        {
            return dao.Facet.Type switch
            {
                FacetType.Integer => dao.ToEnvironmentFacetValue(dao.IntValue.Value),
                FacetType.String => dao.ToEnvironmentFacetValue(dao.StringValue),
                FacetType.DateTime => dao.ToEnvironmentFacetValue(dao.DateTimeValue.Value),
                FacetType.Decimal => dao.ToEnvironmentFacetValue(dao.DecimalValue.Value),
                FacetType.Percentage => dao.ToEnvironmentFacetValue(dao.DecimalValue.Value),
                _ => null,
            };
        }

        private static IEnvironmentFacetValue ToEnvironmentFacetValue<T>(this EnvironmentFacetValueDao dao, T value)
        {
            return new EnvironmentFacetValue<T>
            {
                Id = dao.Id,
                EnvironmentId = dao.EnvironmentId,
                FacetId = dao.FacetId,
                Type = dao.Facet.Type,
                Value = value,
                Facet = new FacetIdentifier
                {
                    Code = dao.Facet.Code,
                    ApplicationId = dao.Facet.ApplicationId
                },
            };
        }
    }
}
