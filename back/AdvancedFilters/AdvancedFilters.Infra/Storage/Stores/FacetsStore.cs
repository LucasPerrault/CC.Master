using System;
using AdvancedFilters.Domain.Facets;
using Lucca.Core.Api.Abstractions.Paging;
using Lucca.Core.Api.Queryable.Paging;
using Microsoft.EntityFrameworkCore;
using Storage.Infra.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AdvancedFilters.Domain.Facets.DAO;
using AdvancedFilters.Domain.Filters.Models;
using Lucca.Core.Shared.Domain.Exceptions;
using Environment = AdvancedFilters.Domain.Instance.Models.Environment;
using AdvancedFilters.Domain.Instance.Models;

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

        public async Task<Page<IEstablishmentFacetValue>> GetValuesAsync(IPageToken pageToken, EstablishmentFacetValueFilter filter)
        {
            var queryable = EstablishmentValues.WhereMatches(filter);
            var page = await _queryPager.ToPageAsync(queryable, pageToken);

            return new Page<IEstablishmentFacetValue>
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

        public async Task<List<IEstablishmentFacetValue>> GetValuesAsync(EstablishmentFacetValueFilter filter)
        {
            return (await EstablishmentValues
                    .WhereMatches(filter)
                    .ToListAsync())
                .ToValues()
                .ToList();
        }

        private IQueryable<Facet> Facets(FacetScope scope) => _dbContext
            .Set<Facet>()
            .Where(f => f.Scope == scope);

        public Expression<Func<Environment, bool>> GetEnvFacetFilter(EnvironmentFacetsAdvancedCriterion criterion)
        {
            var queryable = EnvironmentValuesWithType(criterion.Value.Type).WhereMatches(criterion);

            return e => queryable.Any(dao => e.Id == dao.EnvironmentId);
        }

        public Expression<Func<Establishment, bool>> GetEstablishmentFacetFilter(EstablishmentFacetsAdvancedCriterion criterion)
        {
            var queryable = EstablishmentValuesWithType(criterion.Value.Type).WhereMatches(criterion);

            return e => queryable.Any(dao => e.Id == dao.EnvironmentId);
        }

        private IQueryable<EnvironmentFacetValueDao> EnvironmentValues => _dbContext
            .Set<EnvironmentFacetValueDao>()
            .Where(v => v.Facet.Type != FacetType.Unknown)
            .Include(v => v.Facet);

        private IQueryable<EnvironmentFacetValueDao> EnvironmentValuesWithType(FacetType facetType) => _dbContext
            .Set<EnvironmentFacetValueDao>()
            .Where(v => v.Facet.Type == facetType)
            .Include(v => v.Facet);
        private IQueryable<EstablishmentFacetValueDao> EstablishmentValuesWithType(FacetType facetType) => _dbContext
            .Set<EstablishmentFacetValueDao>()
            .Where(v => v.Facet.Type == facetType)
            .Include(v => v.Facet);


        private IQueryable<EstablishmentFacetValueDao> EstablishmentValues => _dbContext
            .Set<EstablishmentFacetValueDao>()
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
                .WhenNotNullOrEmpty(filter.FacetIds).ApplyWhere(dao => filter.FacetIds.Contains(dao.FacetId))
                .WhenNotNullOrEmpty(filter.Search).ApplyWhere(dao => dao.StringValue.Contains(filter.Search));
        }

        public static IQueryable<EstablishmentFacetValueDao> WhereMatches(this IQueryable<EstablishmentFacetValueDao> daos, EstablishmentFacetValueFilter filter)
        {
            return daos
                .WhenNotNullOrEmpty(filter.Codes).ApplyWhere(dao => filter.Codes.Contains(dao.Facet.Code))
                .WhenNotNullOrEmpty(filter.ApplicationIds).ApplyWhere(dao => filter.ApplicationIds.Contains(dao.Facet.ApplicationId))
                .WhenNotNullOrEmpty(filter.FacetTypes).ApplyWhere(dao => filter.FacetTypes.Contains(dao.Facet.Type))
                .WhenNotNullOrEmpty(filter.EnvironmentIds).ApplyWhere(dao => filter.EnvironmentIds.Contains(dao.EnvironmentId))
                .WhenNotNullOrEmpty(filter.EstablishmentIds).ApplyWhere(dao => filter.EstablishmentIds.Contains(dao.EstablishmentId))
                .WhenNotNullOrEmpty(filter.FacetIds).ApplyWhere(dao => filter.FacetIds.Contains(dao.FacetId))
                .WhenNotNullOrEmpty(filter.Search).ApplyWhere(dao => dao.StringValue.Contains(filter.Search));
        }

        public static IQueryable<EnvironmentFacetValueDao> WhereMatches(this IQueryable<EnvironmentFacetValueDao> daos, EnvironmentFacetAdvancedCriterion criterion)
        {
            return daos
                .WhenNotNullOrEmpty(criterion.Identifier.Code).ApplyWhere(dao => dao.Facet.Code == criterion.Identifier.Code)
                .WhenNotNullOrEmpty(criterion.Identifier.ApplicationId).ApplyWhere(dao => dao.Facet.ApplicationId == criterion.Identifier.ApplicationId)
                .When(criterion.Value.Type != FacetType.Unknown).ApplyWhere(GetEnvironmentFacetValueExpressionAccordingToFacetType(criterion.Value));
        }
        private static Expression<Func<EnvironmentFacetValueDao, bool>> GetEnvironmentFacetValueExpressionAccordingToFacetType(IEnvironmentFacetCriterion value) => value.Type switch
        {
            FacetType.Integer => GetIntValue.Chain(((SingleFacetIntValueComparisonCriterion)value).Expression),
            FacetType.DateTime => GetDateTimeValue.Chain(((SingleFacetDateTimeValueComparisonCriterion)value).Expression),
            FacetType.Decimal => GetDecimalValue.Chain(((SingleFacetDecimalValueComparisonCriterion)value).Expression),
            FacetType.Percentage => GetDecimalValue.Chain(((SingleFacetDecimalValueComparisonCriterion)value).Expression),
            FacetType.String => GetStringValue.Chain(((SingleFacetValueComparisonCriterion<string>)value).Expression),
            _ => throw new BadRequestException()
        };

        public static IQueryable<EstablishmentFacetValueDao> WhereMatches(this IQueryable<EstablishmentFacetValueDao> daos, EstablishmentFacetAdvancedCriterion criterion)
        {
            return daos
                .WhenNotNullOrEmpty(criterion.Identifier.Code).ApplyWhere(dao => dao.Facet.Code == criterion.Identifier.Code)
                .WhenNotNullOrEmpty(criterion.Identifier.ApplicationId).ApplyWhere(dao => dao.Facet.ApplicationId == criterion.Identifier.ApplicationId)
                .When(criterion.Value.Type != FacetType.Unknown).ApplyWhere(GetEstablishmentFacetValueExpressionAccordingToFacetType(criterion.Value));
        }
        private static Expression<Func<EstablishmentFacetValueDao, bool>> GetEstablishmentFacetValueExpressionAccordingToFacetType(IEnvironmentFacetCriterion value) => value.Type switch
        {
            FacetType.Integer => GetEstablishmentIntValue.Chain(((SingleFacetIntValueComparisonCriterion)value).Expression),
            FacetType.DateTime => GetEstablishmentDateTimeValue.Chain(((SingleFacetDateTimeValueComparisonCriterion)value).Expression),
            FacetType.Decimal => GetEstablishmentDecimalValue.Chain(((SingleFacetDecimalValueComparisonCriterion)value).Expression),
            FacetType.Percentage => GetEstablishmentDecimalValue.Chain(((SingleFacetDecimalValueComparisonCriterion)value).Expression),
            FacetType.String => GetEstablishmentStringValue.Chain(((SingleFacetValueComparisonCriterion<string>)value).Expression),
            _ => throw new BadRequestException()
        };

        private static Expression<Func<EnvironmentFacetValueDao, int>> GetIntValue => dao => dao.IntValue.Value;
        private static Expression<Func<EnvironmentFacetValueDao, decimal>> GetDecimalValue => dao => dao.DecimalValue.Value;
        private static Expression<Func<EnvironmentFacetValueDao, string>> GetStringValue => dao => dao.StringValue;
        private static Expression<Func<EnvironmentFacetValueDao, DateTime>> GetDateTimeValue => dao => dao.DateTimeValue.Value;

        private static Expression<Func<EstablishmentFacetValueDao, int>> GetEstablishmentIntValue => dao => dao.IntValue.Value;
        private static Expression<Func<EstablishmentFacetValueDao, decimal>> GetEstablishmentDecimalValue => dao => dao.DecimalValue.Value;
        private static Expression<Func<EstablishmentFacetValueDao, string>> GetEstablishmentStringValue => dao => dao.StringValue;
        private static Expression<Func<EstablishmentFacetValueDao, DateTime>> GetEstablishmentDateTimeValue => dao => dao.DateTimeValue.Value;

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

        public static IEnumerable<IEstablishmentFacetValue> ToValues(this IEnumerable<EstablishmentFacetValueDao> daos)
        {
            return daos.Select(dao => dao.ToValue());
        }

        private static IEstablishmentFacetValue ToValue(this EstablishmentFacetValueDao dao)
        {
            return dao.Facet.Type switch
            {
                FacetType.Integer => dao.ToEstablishmentFacetValue(dao.IntValue.Value),
                FacetType.String => dao.ToEstablishmentFacetValue(dao.StringValue),
                FacetType.DateTime => dao.ToEstablishmentFacetValue(dao.DateTimeValue.Value),
                FacetType.Decimal => dao.ToEstablishmentFacetValue(dao.DecimalValue.Value),
                FacetType.Percentage => dao.ToEstablishmentFacetValue(dao.DecimalValue.Value),
                _ => null,
            };
        }

        private static IEstablishmentFacetValue ToEstablishmentFacetValue<T>(this EstablishmentFacetValueDao dao, T value)
        {
            return new EstablishmentFacetValue<T>
            {
                Id = dao.Id,
                EnvironmentId = dao.EnvironmentId,
                EstablishmentId = dao.EstablishmentId,
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
