using AdvancedFilters.Domain.Filters.Builders;
using AdvancedFilters.Domain.Filters.Models;
using Storage.Infra.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace AdvancedFilters.Infra.Filters.Builders.Chaining
{
    internal static class AdvancedExpressionChainingExtensions
    {
        public static Expression<Func<TItem, bool>> ChainToPropertyList<TItem, TProperty>
        (
            this AdvancedCriterion<TProperty> criterion,
            Expression<Func<TItem, IEnumerable<TProperty>>> expression
        )
        {
            var builder = criterion.GetExpressionBuilder();
            return expression.Chain(ForEachApplyToItem(builder));
        }

        public static Expression<Func<TItem, bool>> ChainToPropertyItem<TItem, TProperty>
        (
            this AdvancedCriterion<TProperty> criterion,
            Expression<Func<TItem, TProperty>> expression
        )
        {
            var builder = criterion.GetExpressionBuilder();
            return expression.Chain(ApplyToItem(builder));
        }

        private static Expression<Func<IEnumerable<TProperty>, bool>> ForEachApplyToItem<TProperty>(IQueryableExpressionBuilder<TProperty> expressionBuilder)
        {
            if (!expressionBuilder.CanBuild())
            {
                return _ => true;
            }

            return expressionBuilder.IntersectionOrBypass;
        }

        private static Expression<Func<TProperty, bool>> ApplyToItem<TProperty>(IQueryableExpressionBuilder<TProperty> expressionBuilder)
        {
            if (!expressionBuilder.CanBuild())
            {
                return _ => true;
            }

            return expressionBuilder.MatchOrBypass;
        }
    }
}
