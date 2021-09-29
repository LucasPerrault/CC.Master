using AdvancedFilters.Domain.Filters.Models;
using AdvancedFilters.Domain.Instance.Models;
using AdvancedFilters.Infra.Filters.Builders;
using Storage.Infra.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace AdvancedFilters.Infra.Filters
{
    internal static class AdvancedFilterItemsQueryableExtensions
    {
        public static ICompareAdvancedFilterItemBuilder<TRoot, AppInstance> Apply<TRoot>(this IQueryable<TRoot> query, AppInstanceAdvancedCriterion criterion)
            => query.Apply(new AppInstanceFiltersExpressionBuilder(criterion));

        public static ICompareAdvancedFilterItemBuilder<TRoot, LegalUnit> Apply<TRoot>(this IQueryable<TRoot> query, LegalUnitAdvancedCriterion criterion)
            => query.Apply(new LegalUnitFiltersExpressionBuilder(criterion));

        private static ICompareAdvancedFilterItemBuilder<TRoot, TItem> Apply<TRoot, TItem, TCriterion>
        (
            this IQueryable<TRoot> query,
            AdvancedFiltersExpressionBuilder<TItem, TCriterion> expressionBuilder
        )
            where TCriterion : AdvancedCriterion
        {
            return new CompareAdvancedFilterItemBuilder<TRoot, TItem>(query, expressionBuilder.IntersectionOrBypass);
        }
    }

    internal interface ICompareAdvancedFilterItemBuilder<TRoot, TItem>
    {
        IQueryable<TRoot> To(Expression<Func<TRoot, IEnumerable<TItem>>> getAppInstancesExpression);
    }

    internal class CompareAdvancedFilterItemBuilder<TRoot, TItem> : ICompareAdvancedFilterItemBuilder<TRoot, TItem>
    {
        private readonly IQueryable<TRoot> _queryable;
        private readonly Expression<Func<IEnumerable<TItem>, bool>> _getItemsComparisonExpression;

        public CompareAdvancedFilterItemBuilder(IQueryable<TRoot> queryable, Expression<Func<IEnumerable<TItem>, bool>> getItemsComparisonExpression)
        {
            _queryable = queryable;
            _getItemsComparisonExpression = getItemsComparisonExpression;
        }

        public IQueryable<TRoot> To(Expression<Func<TRoot, IEnumerable<TItem>>> getComparedItemsExpression)
        {
            return _queryable.Where(getComparedItemsExpression.Chain(_getItemsComparisonExpression));
        }
    }
}
