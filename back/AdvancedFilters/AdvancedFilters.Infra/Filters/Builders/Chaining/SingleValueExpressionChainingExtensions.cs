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

            return criterion.Operator.ShouldApplyToAll()
                ? items => items.AsQueryable().All(criterion.Expression)
                : items => items.AsQueryable().Any(criterion.Expression);
        }

        private static Expression<Func<TProperty, bool>> ApplyToItem<TProperty>(SingleValueComparisonCriterion<TProperty> criterion)
            where TProperty : IConvertible
        {
            if (criterion == null)
            {
                return _ => true;
            }

            return criterion.Expression;
        }

        private static bool ShouldApplyToAll(this ComparisonOperators comparisonOperator)
        {
            return comparisonOperator == ComparisonOperators.NotEquals;
        }
    }
}
