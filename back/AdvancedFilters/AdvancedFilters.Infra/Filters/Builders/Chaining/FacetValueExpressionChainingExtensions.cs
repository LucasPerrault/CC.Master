//using AdvancedFilters.Domain.Facets;
//using AdvancedFilters.Domain.Filters.Models;
//using AdvancedFilters.Infra.Filters.Builders.Exceptions;
//using Storage.Infra.Extensions;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;

//namespace AdvancedFilters.Infra.Filters.Builders.Chaining
//{
//    internal static class FacetValueExpressionChainingExtensions
//    {
//        public static Expression<Func<TItem, bool>> Chain<TItem>
//        (
//            this IEnvironmentFacetCriterion criterion,
//            Expression<Func<TItem, IEnumerable<IEnvironmentFacetValue>>> expression
//        )
//        {
//            return expression.Chain(ApplyToList(criterion));
//        }

//        public static Expression<Func<TItem, bool>> Chain<TItem>
//        (
//            this IEnvironmentFacetCriterion criterion,
//            Expression<Func<TItem, IEnvironmentFacetValue>> expression
//        )
//        {
//            return expression.Chain(ApplyToItem(criterion));
//        }

//        private static Expression<Func<IEnumerable<IEnvironmentFacetValue>, bool>> ApplyToList(IEnvironmentFacetCriterion criterion)
//        {
//            if (criterion == null)
//            {
//                return _ => true;
//            }

//            var itemsMatching = GetItemMatching(criterion);
//            return criterion.GetExpression().ToExpressionForList(itemsMatching);
//        }

//        private static Expression<Func<IEnvironmentFacetValue, bool>> ApplyToItem(IEnvironmentFacetCriterion criterion)
//        {
//            if (criterion == null)
//            {
//                return _ => true;
//            }

//            return criterion.Expression;
//        }

//        private static ItemsMatching GetItemMatching(IEnvironmentFacetCriterion criterion)
//        {
//            if (!(criterion is IListCriterion listCriterion))
//            {
//                throw new MissingItemsMatchedFieldException<IEnvironmentFacetValue>();
//            }

//            return listCriterion.ItemsMatched;
//        }
//    }
//}
