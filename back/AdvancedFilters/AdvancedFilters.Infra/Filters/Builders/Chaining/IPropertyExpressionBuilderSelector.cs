using AdvancedFilters.Domain.Filters.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace AdvancedFilters.Infra.Filters.Builders.Chaining
{
    internal interface IPropertyExpressionBuilder<TValue>
    {
        Expression<Func<IEnumerable<TValue>, bool>> ForList();
        Expression<Func<TValue, bool>> ForItem();
    }

    internal interface IPropertyExpressionBuilderSelector<TValue, TProperty>
    {
        IPropertyExpressionBuilder<TValue> To(Expression<Func<TValue, IEnumerable<TProperty>>> getPropertyListExpression);
        IPropertyExpressionBuilder<TValue> To(Expression<Func<TValue, TProperty>> getPropertyExpression);
    }

    internal class SingleValueExpressionBuilderSelector<TValue, TProperty> : IPropertyExpressionBuilderSelector<TValue, TProperty>
        where TProperty : IConvertible
    {
        private readonly SingleValueComparisonCriterion<TProperty> _criterion;

        public SingleValueExpressionBuilderSelector(SingleValueComparisonCriterion<TProperty> criterion)
        {
            _criterion = criterion;
        }

        public IPropertyExpressionBuilder<TValue> To(Expression<Func<TValue, IEnumerable<TProperty>>> getPropertyListExpression)
            => new SingleValuePropertyListExpressionBuilder<TValue, TProperty>(_criterion, getPropertyListExpression);

        public IPropertyExpressionBuilder<TValue> To(Expression<Func<TValue, TProperty>> getPropertyExpression)
            => new SingleValuePropertyExpressionBuilder<TValue, TProperty>(_criterion, getPropertyExpression);
    }

    internal class AdvancedExpressionBuilderSelector<TValue, TProperty> : IPropertyExpressionBuilderSelector<TValue, TProperty>
    {
        private readonly AdvancedCriterion<TProperty> _criterion;

        public AdvancedExpressionBuilderSelector(AdvancedCriterion<TProperty> criterion)
        {
            _criterion = criterion;
        }

        public IPropertyExpressionBuilder<TValue> To(Expression<Func<TValue, IEnumerable<TProperty>>> getPropertyListExpression)
            => new AdvancedPropertyListExpressionBuilder<TValue, TProperty>(_criterion, getPropertyListExpression);

        public IPropertyExpressionBuilder<TValue> To(Expression<Func<TValue, TProperty>> getPropertyExpression)
            => new AdvancedPropertyExpressionBuilder<TValue, TProperty>(_criterion, getPropertyExpression);
    }
}
