using AdvancedFilters.Domain.Filters.Models;
using AdvancedFilters.Infra.Filters.Builders.Exceptions;
using Storage.Infra.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace AdvancedFilters.Infra.Filters.Builders.Chaining
{
    internal static class SingleValueExpressionChainingExtensions
    {
        public static Expression<Func<TItem, bool>> Chain<TItem, TProperty>
        (
            this SingleValueComparisonCriterion<TProperty> criterion,
            Expression<Func<TItem, IEnumerable<TProperty>>> expression
        )
        {
            return expression.Chain(ApplyToList(criterion));
        }

        public static Expression<Func<TItem, bool>> Chain<TItem, TProperty>
        (
            this SingleValueComparisonCriterion<TProperty> criterion,
            Expression<Func<TItem, TProperty>> expression
        )
        {
            return expression.Chain(ApplyToItem(criterion));
        }

        private static Expression<Func<IEnumerable<TProperty>, bool>> ApplyToList<TProperty>(SingleValueComparisonCriterion<TProperty> criterion)
        {
            if (criterion == null)
            {
                return _ => true;
            }

            var itemsMatching = GetItemMatching(criterion);
            return criterion.Expression.ToExpressionForList(itemsMatching);
        }

        private static Expression<Func<TProperty, bool>> ApplyToItem<TProperty>(SingleValueComparisonCriterion<TProperty> criterion)
        {
            if (criterion == null)
            {
                return _ => true;
            }

            return criterion.Expression;
        }

        private static ItemsMatching GetItemMatching<TProperty>(SingleValueComparisonCriterion<TProperty> criterion)
        {
            if (!(criterion is IListCriterion listCriterion))
            {
                throw new MissingItemsMatchedFieldException<SingleValueComparisonCriterion<TProperty>>();
            }

            return listCriterion.ItemsMatched;
        }
    }
}
