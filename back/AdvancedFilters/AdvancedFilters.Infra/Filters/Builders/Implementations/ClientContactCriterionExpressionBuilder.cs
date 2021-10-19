using AdvancedFilters.Domain.Contacts.Models;
using AdvancedFilters.Infra.Filters.Builders.Chaining;
using System.Collections.Generic;

namespace AdvancedFilters.Infra.Filters.Builders.Implementations
{
    internal class ClientContactCriterionExpressionBuilder : AdvancedCriterionExpressionBuilder<ClientContact, ClientContactAdvancedCriterion>
    {
        public ClientContactCriterionExpressionBuilder(ClientContactAdvancedCriterion criterion)
            : base(criterion)
        { }

        protected override IEnumerable<IPropertyExpressionBuilder<ClientContact>> GetCriteria()
        {
            yield return Apply(Criterion.IsConfirmed).To(c => c.IsConfirmed);
            yield return Apply(Criterion.Environment).To(c => c.Environment);
            yield return Apply(Criterion.LegalUnit).To(c => c.Establishment.LegalUnit);
        }
    }
}
