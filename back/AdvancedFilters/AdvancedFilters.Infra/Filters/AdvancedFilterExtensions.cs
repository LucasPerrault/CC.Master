using AdvancedFilters.Domain.Filters.Models;
using System;
using System.ComponentModel;
using System.Linq;

namespace AdvancedFilters.Infra.Filters
{
    internal interface IAdvancedCriterionApplier<T, TAdvancedCriterion>
    {
        IQueryable<T> Apply(IQueryable<T> queryable, TAdvancedCriterion criterion);
    }

    internal static class AdvanceFilterExtensions
    {
        public static IQueryable<TModel> Filter<TModel, TAdvancedCriterion>
        (
            this IQueryable<TModel> queryable,
            IAdvancedFilter filter,
            IAdvancedCriterionApplier<TModel, TAdvancedCriterion> applier
        )
            where TAdvancedCriterion : AdvancedCriterion
        {
            return filter switch
            {
                FilterCombination combination => queryable.Combine(combination, applier),
                TAdvancedCriterion criterion => applier.Apply(queryable, criterion),
                _ => throw new InvalidOperationException($"Unhandled {nameof(filter)} type {filter.GetType()}")
            };
        }

        private static IQueryable<TModel> Combine<TModel, TAdvancedCriterion>
        (
            this IQueryable<TModel> queryable,
            FilterCombination combination,
            IAdvancedCriterionApplier<TModel, TAdvancedCriterion> applier
        )
            where TAdvancedCriterion : AdvancedCriterion
        {
            return combination.Operator switch
            {
                FilterOperatorTypes.And => combination.Values.Skip(1)
                    .Aggregate(
                        queryable.Filter(combination.Values.First(), applier),
                        (q, f) => q.Intersect(queryable.Filter(f, applier))
                    ),
                FilterOperatorTypes.Or => combination.Values.Skip(1)
                    .Aggregate(
                        queryable.Filter(combination.Values.First(), applier),
                        (q, f) => q.Union(queryable.Filter(f, applier))
                    ),
                _ => throw new InvalidEnumArgumentException(nameof(combination.Operator), (int)combination.Operator, typeof(FilterOperatorTypes))
            };
        }
    }
}
