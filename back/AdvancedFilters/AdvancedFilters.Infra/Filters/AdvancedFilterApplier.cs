using AdvancedFilters.Domain.Filters.Builders;
using AdvancedFilters.Domain.Filters.Models;
using AdvancedFilters.Infra.Filters.Builders;
using Lucca.Core.Shared.Domain.Exceptions;
using System;
using System.ComponentModel;
using System.Linq;

namespace AdvancedFilters.Infra.Filters
{
    public class AdvancedFilterApplier
    {
        private readonly IQueryableExpressionBuilderFactory _factory;

        public AdvancedFilterApplier(IQueryableExpressionBuilderFactory factory)
        {
            _factory = factory;
        }

        public IQueryable<TModel> Filter<TModel>
        (
            IQueryable<TModel> queryable,
            IAdvancedFilter filter
        )
        {
            return filter switch
            {
                FilterCombination combination => Combine(queryable, combination),
                AdvancedCriterion<TModel> criterion => Apply(queryable, criterion),
                _ => throw new InvalidOperationException($"Unhandled {nameof(filter)} type {filter.GetType()}")
            };
        }

        private IQueryable<TModel> Combine<TModel>
        (
            IQueryable<TModel> queryable,
            FilterCombination combination
        )
        {
            if (combination.Values == null || !combination.Values.Any())
            {
                throw new BadRequestException("Filter combination must have at least one value");
            }

            if (combination.Values.Count == 1)
            {
                return Filter(queryable, combination.Values.Single());
            }

            return combination.Operator switch
            {
                FilterOperatorTypes.And => combination.Values.Skip(1)
                    .Aggregate(
                        Filter(queryable, combination.Values.First()),
                        (q, f) => q.Intersect(Filter(queryable, f))
                    ),
                FilterOperatorTypes.Or => combination.Values.Skip(1)
                    .Aggregate(
                        Filter(queryable, combination.Values.First()),
                        (q, f) => q.Union(Filter(queryable, f))
                    ),
                _ => throw new InvalidEnumArgumentException(nameof(combination.Operator), (int)combination.Operator, typeof(FilterOperatorTypes))
            };
        }

        private IQueryable<TModel> Apply<TModel>
        (
            IQueryable<TModel> queryable,
            AdvancedCriterion<TModel> criterion
        )
        {
            var builder = criterion.GetExpressionBuilderOrBypass(_factory);
            return queryable.Where(builder.MatchOrBypass);
        }
    }
}
