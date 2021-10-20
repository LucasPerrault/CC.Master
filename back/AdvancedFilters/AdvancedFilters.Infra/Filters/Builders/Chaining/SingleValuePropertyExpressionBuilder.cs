using AdvancedFilters.Domain.Filters.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace AdvancedFilters.Infra.Filters.Builders.Chaining
{
    internal class SingleValuePropertyExpressionBuilder<TValue, TProperty> : IPropertyExpressionBuilder<TValue>
    {
        private readonly SingleValueComparisonCriterion<TProperty> _criterion;
        private readonly Expression<Func<TValue, TProperty>> _getPropertyExpression;

        public SingleValuePropertyExpressionBuilder(SingleValueComparisonCriterion<TProperty> criterion, Expression<Func<TValue, TProperty>> getPropertyExpression)
        {
            _criterion = criterion;
            _getPropertyExpression = getPropertyExpression;
        }

        public Expression<Func<IEnumerable<TValue>, bool>> ForList()
        {
            return values => values.AsQueryable().Any(_criterion.Chain(_getPropertyExpression));
        }

        public Expression<Func<TValue, bool>> ForItem()
        {
            return _criterion.Chain(_getPropertyExpression);
        }
    }

    internal class SingleValuePropertyListExpressionBuilder<TValue, TProperty> : IPropertyExpressionBuilder<TValue>
    {
        private readonly SingleValueComparisonCriterion<TProperty> _criterion;
        private readonly Expression<Func<TValue, IEnumerable<TProperty>>> _getPropertyListExpression;

        public SingleValuePropertyListExpressionBuilder(SingleValueComparisonCriterion<TProperty> criterion, Expression<Func<TValue, IEnumerable<TProperty>>> getPropertyListExpression)
        {
            _criterion = criterion;
            _getPropertyListExpression = getPropertyListExpression;
        }

        public Expression<Func<IEnumerable<TValue>, bool>> ForList()
        {
            return values => values.AsQueryable().Any(_criterion.Chain(_getPropertyListExpression));
        }

        public Expression<Func<TValue, bool>> ForItem()
        {
            return _criterion.Chain(_getPropertyListExpression);
        }
    }
}
