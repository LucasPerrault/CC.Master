using AdvancedFilters.Domain.Filters.Models;
using AdvancedFilters.Infra.Filters.Builders.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace AdvancedFilters.Infra.Filters.Builders.Chaining
{
    internal interface IPropertyExpressionBuilder<TValue>
    {
        Expression<Func<IEnumerable<TValue>, bool>> ForList(ItemsMatching matching);
        Expression<Func<TValue, bool>> ForItem();
    }

    internal static class PropertyExpressionBuilderExtensions
    {
        public static Expression<Func<IEnumerable<TValue>,bool>> ToExpressionForList<TValue>(this Expression<Func<TValue, bool>> itemPredicate, ItemsMatching matching)
            => matching switch
            {
                ItemsMatching.Any => values => values.AsQueryable().Any(itemPredicate),
                ItemsMatching.All => values => values.Any() && values.AsQueryable().All(itemPredicate),
                _ => throw new MissingItemsMatchedFieldException<TValue>()
            };
    }
}
