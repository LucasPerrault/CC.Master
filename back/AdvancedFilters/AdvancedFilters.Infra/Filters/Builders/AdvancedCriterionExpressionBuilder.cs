using AdvancedFilters.Domain.Facets;
using AdvancedFilters.Domain.Filters.Builders;
using AdvancedFilters.Domain.Filters.Models;
using AdvancedFilters.Infra.Filters.Builders.Chaining;
using AdvancedFilters.Infra.Filters.Builders.Exceptions;
using Storage.Infra.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace AdvancedFilters.Infra.Filters.Builders
{
    internal abstract class AdvancedCriterionExpressionBuilder<TValue, TCriterion> : IQueryableExpressionBuilder<TValue>
        where TCriterion : AdvancedCriterion
    {
        private readonly IAdvancedExpressionChainer _chainer;

        protected TCriterion Criterion { get; }

        public Expression<Func<IEnumerable<TValue>, bool>> IntersectionOrBypass => GetPredicateOrBypassExpression(b => b.ForList(GetItemMatching()));
        public Expression<Func<TValue, bool>> MatchOrBypass => GetPredicateOrBypassExpression(b => b.ForItem());

        protected AdvancedCriterionExpressionBuilder(TCriterion criterion, IAdvancedExpressionChainer chainer)
        {
            Criterion = criterion;
            _chainer = chainer;
        }

        public virtual bool CanBuild() => Criterion != null;

        protected abstract IEnumerable<IPropertyExpressionBuilder<TValue>> GetCriteria();

        protected IPropertyExpressionBuilderSelector<TValue, TProperty> Apply<TProperty>(SingleValueComparisonCriterion<TProperty> criterion)
            => new SingleValueExpressionBuilderSelector<TValue, TProperty>(criterion);
        protected IPropertyExpressionBuilderSelector<TValue, TProperty> Apply<TProperty>(AdvancedCriterion<TProperty> criterion)
            => new AdvancedExpressionBuilderSelector<TValue, TProperty>(criterion, _chainer);
        protected IPropertyExpressionBuilderSelector<TValue, IEnvironmentFacetValue> Apply(IEnvironmentFacetCriterion criterion)
            => new FacetValueExpressionBuilderSelector<TValue>(criterion);


        protected IPropertyListExpressionBuilderSelector<TValue, TProperty> ApplyMany<TProperty>(SingleValueComparisonCriterion<TProperty> criterion)
            => new SingleValueListExpressionBuilderSelector<TValue, TProperty>(criterion);
        protected IPropertyListExpressionBuilderSelector<TValue, TProperty> ApplyMany<TProperty>(AdvancedCriterion<TProperty> criterion)
            => new AdvancedListExpressionBuilderSelector<TValue, TProperty>(criterion, _chainer);
        protected IPropertyListExpressionBuilderSelector<TValue, IEnvironmentFacetValue> ApplyMany(IEnvironmentFacetCriterion criterion)
            => new FacetValueListExpressionBuilderSelector<TValue>(criterion); // TODO

        private ItemsMatching GetItemMatching()
        {
            if (!(Criterion is IListCriterion listCriterion))
            {
                throw new MissingItemsMatchedFieldException<TCriterion>();
            }

            return listCriterion.ItemsMatched;
        }

        private Expression<Func<TInput, bool>> GetPredicateOrBypassExpression<TInput>(Func<IPropertyExpressionBuilder<TValue>, Expression<Func<TInput, bool>>> buildExpressionFn)
            => CanBuild()
                ? GetCriteria().Select(buildExpressionFn).ToArray().CombineSafelyAnd()
                : _ => true;
    }

    internal class BypassExpressionBuilder<TValue> : IQueryableExpressionBuilder<TValue>
    {
        public Expression<Func<IEnumerable<TValue>, bool>> IntersectionOrBypass { get; }
        public Expression<Func<TValue, bool>> MatchOrBypass { get; }

        public bool CanBuild()
        {
            return false;
        }
    }
}
