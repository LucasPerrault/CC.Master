using AdvancedFilters.Domain.Contacts.Models;
using AdvancedFilters.Infra.Filters.Builders.Chaining;
using System.Collections.Generic;

namespace AdvancedFilters.Infra.Filters.Builders.Implementations
{
    internal class SpecializedContactCriterionExpressionBuilder : AdvancedCriterionExpressionBuilder<SpecializedContact, SpecializedContactAdvancedCriterion>
    {
        public SpecializedContactCriterionExpressionBuilder(SpecializedContactAdvancedCriterion criterion)
            : base(criterion)
        { }

        protected override IEnumerable<IPropertyExpressionBuilder<SpecializedContact>> GetCriteria()
        {
            yield return Apply(Criterion.Environment).To(i => i.Environment);
            yield return Apply(Criterion.LegalUnit).To(i => i.Establishment.LegalUnit);
        }
    }
}
