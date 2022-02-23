using AdvancedFilters.Domain.Facets;
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

    internal class FacetValueExpressionBuilderSelector<TValue> : IPropertyExpressionBuilderSelector<TValue, IEnvironmentFacetValue>
    {
        private readonly IEnvironmentFacetCriterion _criterion;

        public FacetValueExpressionBuilderSelector(IEnvironmentFacetCriterion criterion)
        {
            _criterion = criterion;
        }

        public IPropertyExpressionBuilder<TValue> To(Expression<Func<TValue, IEnvironmentFacetValue>> getPropertyExpression)
            => new FacetValuePropertyExpressionBuilder<TValue>(_criterion, getPropertyExpression);
    }

    internal class FacetValueListExpressionBuilderSelector<TValue> : IPropertyListExpressionBuilderSelector<TValue, IEnvironmentFacetValue>
    {
        private readonly IEnvironmentFacetCriterion _criterion;

        public FacetValueListExpressionBuilderSelector(IEnvironmentFacetCriterion criterion)
        {
            _criterion = criterion;
        }

        public IPropertyExpressionBuilder<TValue> To(Expression<Func<TValue, IEnumerable<IEnvironmentFacetValue>>> getPropertyListExpression)
            => new FacetValuePropertyListExpressionBuilder<TValue>(_criterion, getPropertyListExpression);
    }

    internal class AdvancedExpressionBuilderSelector<TValue, TProperty> : IPropertyExpressionBuilderSelector<TValue, TProperty>
    {
        private readonly AdvancedCriterion<TProperty> _criterion;
        private readonly IAdvancedExpressionChainer _chainer;

        public AdvancedExpressionBuilderSelector(AdvancedCriterion<TProperty> criterion, IAdvancedExpressionChainer chainer)
        {
            _criterion = criterion;
            _chainer = chainer;
        }

        public IPropertyExpressionBuilder<TValue> To(Expression<Func<TValue, TProperty>> getPropertyExpression)
            => new AdvancedPropertyExpressionBuilder<TValue, TProperty>(_criterion, getPropertyExpression, _chainer);
    }

    internal class AdvancedListExpressionBuilderSelector<TValue, TProperty> : IPropertyListExpressionBuilderSelector<TValue, TProperty>
    {
        private readonly AdvancedCriterion<TProperty> _criterion;
        private readonly IAdvancedExpressionChainer _chainer;

        public AdvancedListExpressionBuilderSelector(AdvancedCriterion<TProperty> criterion, IAdvancedExpressionChainer chainer)
        {
            _criterion = criterion;
            _chainer = chainer;
        }

        public IPropertyExpressionBuilder<TValue> To(Expression<Func<TValue, IEnumerable<TProperty>>> getPropertyListExpression)
            => new AdvancedPropertyListExpressionBuilder<TValue, TProperty>(_criterion, getPropertyListExpression, _chainer);
    }
}
