using AdvancedFilters.Domain.Filters.Models;
using Storage.Infra.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace AdvancedFilters.Infra.Filters.Builders.Chaining
{
    internal static class SingleValueExpressionChainingExtensions
    {
        public static Expression<Func<IEnumerable<TItem>, bool>> Chain<TItem, TProperty>
        (
            this SingleValueComparisonCriterion<TProperty> criterion,
            Expression<Func<IEnumerable<TItem>, IEnumerable<TProperty>>> expression
        )
            where TProperty : IConvertible
        {
            return expression.Chain(ApplyToList(criterion));
        }

        public static Expression<Func<TItem, bool>> Chain<TItem, TProperty>
        (
            this SingleValueComparisonCriterion<TProperty> criterion,
            Expression<Func<TItem, IEnumerable<TProperty>>> expression
        )
            where TProperty : IConvertible
        {
            return expression.Chain(ApplyToList(criterion));
        }

        public static Expression<Func<TItem, bool>> Chain<TItem, TProperty>
        (
            this SingleValueComparisonCriterion<TProperty> criterion,
            Expression<Func<TItem, TProperty>> expression
        )
            where TProperty : IConvertible
        {
            return expression.Chain(ApplyToItem(criterion));
        }

        private static Expression<Func<IEnumerable<TProperty>, bool>> ApplyToList<TProperty>(SingleValueComparisonCriterion<TProperty> criterion)
            where TProperty : IConvertible
        {
            if (criterion == null)
            {
                return _ => true;
            }

            return criterion.Operator switch
            {
                ComparisonOperators.Equals => items => items.Contains(criterion.Value),
                ComparisonOperators.NotEquals => items => !items.Contains(criterion.Value),
                _ => throw new InvalidEnumArgumentException(nameof(criterion.Operator), (int)criterion.Operator, typeof(ComparisonOperators)),
            };
        }

        private static Expression<Func<TProperty, bool>> ApplyToItem<TProperty>(SingleValueComparisonCriterion<TProperty> criterion)
            where TProperty : IConvertible
        {
            if (criterion == null)
            {
                return _ => true;
            }

            return criterion.Operator switch
            {
                ComparisonOperators.Equals => item => Equals(item, criterion.Value),
                ComparisonOperators.NotEquals => item => !Equals(item, criterion.Value),
                _ => throw new InvalidEnumArgumentException(nameof(criterion.Operator), (int)criterion.Operator, typeof(ComparisonOperators)),
            };
        }
    }
}
