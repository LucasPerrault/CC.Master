using AdvancedFilters.Domain.Filters.Models;
using Storage.Infra.Extensions;
using System.ComponentModel;
using System.Linq;
using Tools;

namespace AdvancedFilters.Infra.Filters
{
    internal static class SingleValueComparisonQueryableExtensions
    {
        public static ICompareIntQueryableBuilder<T> Apply<T>(this IQueryable<T> query, SingleValueComparisonCriterion<int> comparison)
        {
            var compareString = ConvertToCompareInt(comparison);
            return query.Apply(compareString);
        }

        public static ICompareStringQueryableBuilder<T> Apply<T>(this IQueryable<T> query, SingleValueComparisonCriterion<string> comparison)
        {
            var compareString = ConvertToCompareString(comparison);
            return query.Apply(compareString);
        }

        private static CompareInt ConvertToCompareInt(SingleValueComparisonCriterion<int> comparison)
        {
            if (comparison == null)
            {
                return CompareInt.Bypass;
            }

            return comparison.Operator switch
            {
                ComparisonOperators.Equals => CompareInt.Equals(comparison.Value),
                ComparisonOperators.NotEquals => CompareInt.DoesNotEqual(comparison.Value),
                _ => throw new InvalidEnumArgumentException(nameof(comparison.Operator), (int)comparison.Operator, typeof(ComparisonOperators)),
            };
        }

        private static CompareString ConvertToCompareString(SingleValueComparisonCriterion<string> comparison)
        {
            if (comparison == null)
            {
                return CompareString.Bypass;
            }

            return comparison.Operator switch
            {
                ComparisonOperators.Equals => CompareString.Equals(comparison.Value),
                ComparisonOperators.NotEquals => CompareString.DoesNotEqual(comparison.Value),
                _ => throw new InvalidEnumArgumentException(nameof(comparison.Operator), (int)comparison.Operator, typeof(ComparisonOperators)),
            };
        }
    }
}
