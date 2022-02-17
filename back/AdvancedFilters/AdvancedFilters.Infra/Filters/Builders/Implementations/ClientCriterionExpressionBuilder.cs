using AdvancedFilters.Infra.Filters.Builders.Chaining;
using System.Collections.Generic;
using AdvancedFilters.Domain.Billing.Models;

namespace AdvancedFilters.Infra.Filters.Builders.Implementations
{
    internal class ClientCriterionExpressionBuilder : AdvancedCriterionExpressionBuilder<Client, ClientAdvancedCriterion>
    {
        public ClientCriterionExpressionBuilder(ClientAdvancedCriterion criterion)
            : base(criterion)
        { }

        protected override IEnumerable<IPropertyExpressionBuilder<Client>> GetCriteria()
        {
            yield return Apply(Criterion.BillingEntity).To(e => e.BillingEntity);
        }
    }
}
