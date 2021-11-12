using AdvancedFilters.Infra.Filters.Builders;
using Storage.Infra.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace AdvancedFilters.Infra.Filters
{
    internal interface ICompareAdvancedFilterItemBuilder<TRoot, TItem>
    {
        IQueryable<TRoot> To(Expression<Func<TRoot, IEnumerable<TItem>>> getItemsComparisonExpression);
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
