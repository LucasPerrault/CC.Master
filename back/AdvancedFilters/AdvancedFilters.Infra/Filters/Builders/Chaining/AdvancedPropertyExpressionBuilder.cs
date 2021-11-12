using AdvancedFilters.Domain.Filters.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace AdvancedFilters.Infra.Filters.Builders.Chaining
{
    internal class AdvancedPropertyExpressionBuilder<TValue, TProperty> : IPropertyExpressionBuilder<TValue>
    {
        private AdvancedCriterion<TProperty> _criterion;
        private Expression<Func<TValue, TProperty>> _getPropertyExpression;

        public AdvancedPropertyExpressionBuilder(AdvancedCriterion<TProperty> criterion, Expression<Func<TValue, TProperty>> getPropertyExpression)
        {
            _criterion = criterion;
            _getPropertyExpression = getPropertyExpression;
        }

        public Expression<Func<IEnumerable<TValue>, bool>> ForList(ItemsMatching matching)
        {
            var predicate = _criterion.ChainToPropertyItem(_getPropertyExpression);
            return predicate.ToExpressionForList(matching);
        }

        public Expression<Func<TValue, bool>> ForItem()
        {
            return _criterion.ChainToPropertyItem(_getPropertyExpression);
        }
    }

    internal class AdvancedPropertyListExpressionBuilder<TValue, TProperty> : IPropertyExpressionBuilder<TValue>
    {
        private AdvancedCriterion<TProperty> _criterion;
        private Expression<Func<TValue, IEnumerable<TProperty>>> _getPropertyListExpression;

        public AdvancedPropertyListExpressionBuilder(AdvancedCriterion<TProperty> criterion, Expression<Func<TValue, IEnumerable<TProperty>>> getPropertyListExpression)
        {
            _criterion = criterion;
            _getPropertyListExpression = getPropertyListExpression;
        }

        public Expression<Func<IEnumerable<TValue>, bool>> ForList(ItemsMatching matching)
        {
            var predicate = _criterion.ChainToPropertyList(_getPropertyListExpression);
            return predicate.ToExpressionForList(matching);
        }

        public Expression<Func<TValue, bool>> ForItem()
        {
            return _criterion.ChainToPropertyList(_getPropertyListExpression);
        }
    }
}
