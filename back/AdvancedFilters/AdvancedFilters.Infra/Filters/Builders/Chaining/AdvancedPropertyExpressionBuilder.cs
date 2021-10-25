using AdvancedFilters.Domain.Filters.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public Expression<Func<IEnumerable<TValue>, bool>> ForList()
        {
            return values => values.AsQueryable().Any(_criterion.ChainToPropertyItem(_getPropertyExpression));
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

        public Expression<Func<IEnumerable<TValue>, bool>> ForList()
        {
            return values => values.AsQueryable().Any(_criterion.ChainToPropertyItem(_getPropertyListExpression));
        }

        public Expression<Func<TValue, bool>> ForItem()
        {
            return _criterion.ChainToPropertyItem(_getPropertyListExpression);
        }
    }
}
