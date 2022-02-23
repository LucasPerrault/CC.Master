using AdvancedFilters.Domain.Instance.Models;
using AdvancedFilters.Infra.Filters.Builders.Chaining;
using System.Collections.Generic;

namespace AdvancedFilters.Infra.Filters.Builders.Implementations
{
    internal class AppInstanceCriterionExpressionBuilder : AdvancedCriterionExpressionBuilder<AppInstance, AppInstanceAdvancedCriterion>
    {
        public AppInstanceCriterionExpressionBuilder(AppInstanceAdvancedCriterion criterion, IAdvancedExpressionChainer chainer)
            : base(criterion, chainer)
        { }

        protected override IEnumerable<IPropertyExpressionBuilder<AppInstance>> GetCriteria()
        {
            yield return Apply(Criterion.ApplicationId).To(i => i.ApplicationId);
        }
    }
}
