using AdvancedFilters.Domain.Filters.Models;
using Storage.Infra.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace AdvancedFilters.Infra.Filters.Builders
{
    internal abstract class AdvancedFiltersExpressionBuilder<TValue, TCriterion> where TCriterion : AdvancedCriterion
    {
        protected TCriterion Criterion { get; }

        public AdvancedFiltersExpressionBuilder(TCriterion criterion)
        {
            Criterion = criterion;
        }

        public Expression<Func<IEnumerable<TValue>, bool>> IntersectionOrBypass
            => CanBuild()
                ? Build().ToArray().CombineSafelyAnd()
                : _ => true;

        public abstract IEnumerable<Expression<Func<IEnumerable<TValue>, bool>>> Build();

        public virtual bool CanBuild()
        {
            return Criterion != null;
        }
    }

    internal static class AdvancedFiltersExpressionBuilderExtensions
    {
        public static Expression<Func<IEnumerable<TItem>, bool>> Chain<TItem, TProperty>
        (
            this SingleValueComparisonCriterion<TProperty> criterion,
            Expression<Func<IEnumerable<TItem>, IEnumerable<TProperty>>> expression
        )
            where TProperty : IConvertible
        {
            return expression.Chain(GetSingleValueCriterion(criterion));
        }

        private static Expression<Func<IEnumerable<TProperty>, bool>> GetSingleValueCriterion<TProperty>(SingleValueComparisonCriterion<TProperty> criterion)
            where TProperty : IConvertible
        {
            if (criterion == null)
            {
                return _ => true;
            }

            return criterion.Operator switch
            {
                ComparisonOperators.Equals => items => items.Contains(criterion.Value),
                ComparisonOperators.NotEquals => items => !items.Contains(criterion.Value),
                _ => throw new InvalidEnumArgumentException(nameof(criterion.Operator), (int)criterion.Operator, typeof(ComparisonOperators)),
            };
        }
    }
}
