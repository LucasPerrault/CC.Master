using AdvancedFilters.Infra.Filters.Builders.Chaining;
using System.Collections.Generic;
using AdvancedFilters.Domain.Billing.Models;

namespace AdvancedFilters.Infra.Filters.Builders.Implementations
{
    internal class ContractCriterionExpressionBuilder : AdvancedCriterionExpressionBuilder<Contract, ContractAdvancedCriterion>
    {
        public ContractCriterionExpressionBuilder(ContractAdvancedCriterion criterion)
            : base(criterion)
        { }

        protected override IEnumerable<IPropertyExpressionBuilder<Contract>> GetCriteria()
        {
            yield return Apply(Criterion.Client).To(e => e.Client);
        }
    }
}
