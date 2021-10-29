using AdvancedFilters.Domain.Instance.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace AdvancedFilters.Infra.Filters.Builders
{
    internal class AppInstanceFiltersExpressionBuilder : AdvancedFiltersExpressionBuilder<AppInstance, AppInstanceAdvancedCriterion>
    {
        public AppInstanceFiltersExpressionBuilder(AppInstanceAdvancedCriterion criterion)
            : base(criterion)
        { }

        public override IEnumerable<Expression<Func<IEnumerable<AppInstance>, bool>>> Build()
        {
            yield return Criterion.ApplicationId.Chain<AppInstance, string>(instances => instances.Select(i => i.ApplicationId));
        }
    }
}
