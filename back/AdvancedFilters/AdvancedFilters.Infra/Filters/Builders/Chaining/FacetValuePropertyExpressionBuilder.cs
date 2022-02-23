using AdvancedFilters.Domain.Facets;
using AdvancedFilters.Domain.Filters.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace AdvancedFilters.Infra.Filters.Builders.Chaining
{
    internal class FacetValuePropertyExpressionBuilder<TValue> : IPropertyExpressionBuilder<TValue>
    {
        private readonly IEnvironmentFacetCriterion _criterion;
        private readonly Expression<Func<TValue, IEnvironmentFacetValue>> _getPropertyExpression;

        public FacetValuePropertyExpressionBuilder(IEnvironmentFacetCriterion criterion, Expression<Func<TValue, IEnvironmentFacetValue>> getPropertyExpression)
        {
            _criterion = criterion;
            _getPropertyExpression = getPropertyExpression;
        }

        public Expression<Func<IEnumerable<TValue>, bool>> ForList(ItemsMatching matching)
        {
            //var predicate = _criterion.Chain(_getPropertyExpression);
            //return predicate.ToExpressionForList(matching);
            return null;
        }

        public Expression<Func<TValue, bool>> ForItem()
        {
            //return _criterion.Chain(_getPropertyExpression);
            return null ;
        }
    }

    internal class FacetValuePropertyListExpressionBuilder<TValue> : IPropertyExpressionBuilder<TValue>
    {
        private readonly IEnvironmentFacetCriterion _criterion;
        private readonly Expression<Func<TValue, IEnumerable<IEnvironmentFacetValue>>> _getPropertyListExpression;

        public FacetValuePropertyListExpressionBuilder(IEnvironmentFacetCriterion criterion, Expression<Func<TValue, IEnumerable<IEnvironmentFacetValue>>> getPropertyListExpression)
        {
            _criterion = criterion;
            _getPropertyListExpression = getPropertyListExpression;
        }

        public Expression<Func<IEnumerable<TValue>, bool>> ForList(ItemsMatching matching)
        {
            //var predicate = _criterion.Chain(_getPropertyListExpression);
            //return predicate.ToExpressionForList(matching);
            return null;
        }

        public Expression<Func<TValue, bool>> ForItem()
        {
            return null;
            //return _criterion.Chain(_getPropertyListExpression);
        }
    }
}
