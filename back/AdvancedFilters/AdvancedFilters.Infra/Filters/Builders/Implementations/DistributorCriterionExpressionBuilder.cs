using AdvancedFilters.Domain.Billing.Models;
using AdvancedFilters.Infra.Filters.Builders.Chaining;
using System.Collections.Generic;

namespace AdvancedFilters.Infra.Filters.Builders.Implementations
{
    internal class DistributorCriterionExpressionBuilder : AdvancedCriterionExpressionBuilder<Distributor, DistributorAdvancedCriterion>
    {
        public DistributorCriterionExpressionBuilder(DistributorAdvancedCriterion criterion, IAdvancedExpressionChainer chainer)
            : base(criterion, chainer)
        { }

        protected override IEnumerable<IPropertyExpressionBuilder<Distributor>> GetCriteria()
        {
            yield return Apply(Criterion.Id).To(d => d.Id);
        }
    }
}
