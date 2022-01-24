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

        public Task<Page<IEnvironmentFacetValue>> GetAsync(IPageToken pageToken, EnvironmentFacetFilter filter)
        {
            var queryable = EnvironmentValues.WhereMatches(filter).ToValues();

            return _queryPager.ToPageAsync(queryable, pageToken);
        }

        public Task<List<IEnvironmentFacetValue>> GetAsync(EnvironmentFacetFilter filter)
        {
            return EnvironmentValues
                .WhereMatches(filter)
                .ToValues().ToListAsync();
        }

        private IQueryable<EnvironmentFacetValueDao> EnvironmentValues => _dbContext
            .Set<EnvironmentFacetValueDao>()
            .Where(v => v.Facet.Type != FacetType.Unknown)
            .Include(v => v.Facet);
    }

    internal static class EnvironmentFacetsStoreExtensions
    {
        public static IQueryable<IEnvironmentFacetValue> ToValues(this IQueryable<EnvironmentFacetValueDao> daos)
        {
            return daos.Select(dao => dao.ToValue());
        }
        public static IQueryable<EnvironmentFacetValueDao> WhereMatches(this IQueryable<EnvironmentFacetValueDao> daos, EnvironmentFacetFilter filter)
        {
            return daos
                .WhenNotNullOrEmpty(filter.Codes).ApplyWhere(dao => filter.Codes.Contains(dao.Facet.Code))
                .WhenNotNullOrEmpty(filter.ApplicationIds).ApplyWhere(dao => filter.ApplicationIds.Contains(dao.Facet.ApplicationId))
                .WhenNotNullOrEmpty(filter.FacetTypes).ApplyWhere(dao => filter.FacetTypes.Contains(dao.Facet.Type))
                .WhenNotNullOrEmpty(filter.EnvironmentIds).ApplyWhere(dao => filter.EnvironmentIds.Contains(dao.EnvironmentId))
                .WhenNotNullOrEmpty(filter.FacetIdentifiers).ApplyWhere(dao => filter.FacetIdentifiers.Any(id => dao.Facet.ApplicationId == id.ApplicationId && dao.Facet.Code == id.Code));
        }

        public static IEnvironmentFacetValue ToValue(this EnvironmentFacetValueDao dao)
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
        public static IEnvironmentFacetValue ToEnvironmentFacetValue<T>(this EnvironmentFacetValueDao dao, T value)
        {
            return new EnvironmentFacetValue<T>
            {
                EnvironmentId = dao.EnvironmentId,
                FacetId = dao.FacetId,
                Type = dao.Facet.Type,
                Value = value
            };
        }
    }
}
