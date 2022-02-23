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
        private readonly IAdvancedExpressionChainer _chainer;

        public AdvancedPropertyExpressionBuilder(AdvancedCriterion<TProperty> criterion, Expression<Func<TValue, TProperty>> getPropertyExpression, IAdvancedExpressionChainer chainer)
        {
            _criterion = criterion;
            _getPropertyExpression = getPropertyExpression;
            _chainer = chainer;
        }

        public Expression<Func<IEnumerable<TValue>, bool>> ForList(ItemsMatching matching)
        {
            var predicate = _chainer.ChainToPropertyItem(_criterion, _getPropertyExpression);
            return predicate.ToExpressionForList(matching);
        }

        public Expression<Func<TValue, bool>> ForItem()
        {
            return _chainer.ChainToPropertyItem(_criterion, _getPropertyExpression);
        }
    }

    internal class AdvancedPropertyListExpressionBuilder<TValue, TProperty> : IPropertyExpressionBuilder<TValue>
    {
        private AdvancedCriterion<TProperty> _criterion;
        private Expression<Func<TValue, IEnumerable<TProperty>>> _getPropertyListExpression;
        private readonly IAdvancedExpressionChainer _chainer;

        public AdvancedPropertyListExpressionBuilder(AdvancedCriterion<TProperty> criterion, Expression<Func<TValue, IEnumerable<TProperty>>> getPropertyListExpression, IAdvancedExpressionChainer chainer)
        {
            _criterion = criterion;
            _getPropertyListExpression = getPropertyListExpression;
            _chainer = chainer;
        }

        public Expression<Func<IEnumerable<TValue>, bool>> ForList(ItemsMatching matching)
        {
            var predicate = _chainer.ChainToPropertyList(_criterion, _getPropertyListExpression);
            return predicate.ToExpressionForList(matching);
        }

        public Expression<Func<TValue, bool>> ForItem()
        {
            return _chainer.ChainToPropertyList(_criterion, _getPropertyListExpression);
        }
    }
}
