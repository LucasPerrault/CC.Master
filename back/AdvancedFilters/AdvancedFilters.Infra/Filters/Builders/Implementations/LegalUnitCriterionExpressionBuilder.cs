using AdvancedFilters.Domain.Instance.Models;
using AdvancedFilters.Infra.Filters.Builders.Chaining;
using System.Collections.Generic;

namespace AdvancedFilters.Infra.Filters.Builders.Implementations
{
    internal class LegalUnitCriterionExpressionBuilder : AdvancedCriterionExpressionBuilder<LegalUnit, LegalUnitAdvancedCriterion>
    {
        public LegalUnitCriterionExpressionBuilder(LegalUnitAdvancedCriterion criterion)
            : base(criterion)
        { }

        protected override IEnumerable<IPropertyExpressionBuilder<LegalUnit>> GetCriteria()
        {
            yield return Apply(Criterion.CountryId).To(e => e.CountryId);
        }
    }
}
