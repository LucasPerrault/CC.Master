using AdvancedFilters.Domain.Filters.Models;
using AdvancedFilters.Infra.Filters.Builders;
using System;
using System.ComponentModel;
using System.Linq;

namespace AdvancedFilters.Infra.Filters
{
    internal static class AdvanceFilterExtensions
    {
        public static IQueryable<TModel> Filter<TModel>
        (
            this IQueryable<TModel> queryable,
            IAdvancedFilter filter
        )
        {
            return filter switch
            {
                FilterCombination combination => queryable.Combine(combination),
                AdvancedCriterion<TModel> criterion => queryable.Apply(criterion),
                _ => throw new InvalidOperationException($"Unhandled {nameof(filter)} type {filter.GetType()}")
            };
        }

        private static IQueryable<TModel> Combine<TModel>
        (
            this IQueryable<TModel> queryable,
            FilterCombination combination
        )
        {
            return combination.Operator switch
            {
                FilterOperatorTypes.And => combination.Values.Skip(1)
                    .Aggregate(
                        queryable.Filter(combination.Values.First()),
                        (q, f) => q.Intersect(queryable.Filter(f))
                    ),
                FilterOperatorTypes.Or => combination.Values.Skip(1)
                    .Aggregate(
                        queryable.Filter(combination.Values.First()),
                        (q, f) => q.Union(queryable.Filter(f))
                    ),
                _ => throw new InvalidEnumArgumentException(nameof(combination.Operator), (int)combination.Operator, typeof(FilterOperatorTypes))
            };
        }

        private static IQueryable<TModel> Apply<TModel>
        (
            this IQueryable<TModel> queryable,
            AdvancedCriterion<TModel> criterion
        )
        {
            var builder = criterion.GetExpressionBuilder();
            return queryable.Where(builder.MatchOrBypass);
        }
    }
}
