using AdvancedFilters.Domain.Filters.Builders;
using AdvancedFilters.Domain.Filters.Models;
using AdvancedFilters.Infra.Filters.Builders.Chaining;
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
        protected TCriterion Criterion { get; }

        public Expression<Func<IEnumerable<TValue>, bool>> IntersectionOrBypass => GetPredicateOrBypassExpression(b => b.ForList());
        public Expression<Func<TValue, bool>> MatchOrBypass => GetPredicateOrBypassExpression(b => b.ForItem());

        protected AdvancedCriterionExpressionBuilder(TCriterion criterion)
        {
            Criterion = criterion;
        }

        public virtual bool CanBuild() => Criterion != null;

        protected abstract IEnumerable<IPropertyExpressionBuilder<TValue>> GetCriteria();

        protected IPropertyExpressionBuilderSelector<TValue, TProperty> Apply<TProperty>(SingleValueComparisonCriterion<TProperty> criterion) where TProperty : IConvertible
            => new SingleValueExpressionBuilderSelector<TValue, TProperty>(criterion);
        protected IPropertyExpressionBuilderSelector<TValue, TProperty> Apply<TProperty>(AdvancedCriterion<TProperty> criterion)
            => new AdvancedExpressionBuilderSelector<TValue, TProperty>(criterion);

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
