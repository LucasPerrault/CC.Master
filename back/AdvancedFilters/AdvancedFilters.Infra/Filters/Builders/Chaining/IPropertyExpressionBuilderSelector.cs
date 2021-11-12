using AdvancedFilters.Domain.Filters.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace AdvancedFilters.Infra.Filters.Builders.Chaining
{
    internal interface IPropertyExpressionBuilderSelector<TValue, TProperty>
    {
        IPropertyExpressionBuilder<TValue> To(Expression<Func<TValue, TProperty>> getPropertyExpression);
    }

    internal interface IPropertyListExpressionBuilderSelector<TValue, TProperty>
    {
        IPropertyExpressionBuilder<TValue> To(Expression<Func<TValue, IEnumerable<TProperty>>> getPropertyListExpression);
    }

    internal class SingleValueExpressionBuilderSelector<TValue, TProperty> : IPropertyExpressionBuilderSelector<TValue, TProperty>
    {
        private readonly SingleValueComparisonCriterion<TProperty> _criterion;

        public SingleValueExpressionBuilderSelector(SingleValueComparisonCriterion<TProperty> criterion)
        {
            _criterion = criterion;
        }

        public IPropertyExpressionBuilder<TValue> To(Expression<Func<TValue, TProperty>> getPropertyExpression)
            => new SingleValuePropertyExpressionBuilder<TValue, TProperty>(_criterion, getPropertyExpression);
    }

    internal class SingleValueListExpressionBuilderSelector<TValue, TProperty> : IPropertyListExpressionBuilderSelector<TValue, TProperty>
    {
        private readonly SingleValueComparisonCriterion<TProperty> _criterion;

        public SingleValueListExpressionBuilderSelector(SingleValueComparisonCriterion<TProperty> criterion)
        {
            _criterion = criterion;
        }

        public IPropertyExpressionBuilder<TValue> To(Expression<Func<TValue, IEnumerable<TProperty>>> getPropertyListExpression)
            => new SingleValuePropertyListExpressionBuilder<TValue, TProperty>(_criterion, getPropertyListExpression);
    }

    internal class AdvancedExpressionBuilderSelector<TValue, TProperty> : IPropertyExpressionBuilderSelector<TValue, TProperty>
    {
        private readonly AdvancedCriterion<TProperty> _criterion;

        public AdvancedExpressionBuilderSelector(AdvancedCriterion<TProperty> criterion)
        {
            _criterion = criterion;
        }

        public IPropertyExpressionBuilder<TValue> To(Expression<Func<TValue, TProperty>> getPropertyExpression)
            => new AdvancedPropertyExpressionBuilder<TValue, TProperty>(_criterion, getPropertyExpression);
    }

    internal class AdvancedListExpressionBuilderSelector<TValue, TProperty> : IPropertyListExpressionBuilderSelector<TValue, TProperty>
    {
        private readonly AdvancedCriterion<TProperty> _criterion;

        public AdvancedListExpressionBuilderSelector(AdvancedCriterion<TProperty> criterion)
        {
            _criterion = criterion;
        }

        public IPropertyExpressionBuilder<TValue> To(Expression<Func<TValue, IEnumerable<TProperty>>> getPropertyListExpression)
            => new AdvancedPropertyListExpressionBuilder<TValue, TProperty>(_criterion, getPropertyListExpression);
    }
}
