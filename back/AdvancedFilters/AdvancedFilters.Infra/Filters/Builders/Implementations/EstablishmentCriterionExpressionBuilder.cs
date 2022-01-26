using AdvancedFilters.Domain.Instance.Models;
using AdvancedFilters.Infra.Filters.Builders.Chaining;
using System.Collections.Generic;

namespace AdvancedFilters.Infra.Filters.Builders.Implementations
{
    internal class EstablishmentCriterionExpressionBuilder : AdvancedCriterionExpressionBuilder<Establishment, EstablishmentAdvancedCriterion>
    {
        public EstablishmentCriterionExpressionBuilder(EstablishmentAdvancedCriterion criterion)
            : base(criterion)
        { }

        protected override IEnumerable<IPropertyExpressionBuilder<Establishment>> GetCriteria()
        {
            yield return Apply(Criterion.Environment).To(e => e.Environment);
            yield return Apply(Criterion.LegalUnit).To(e => e.LegalUnit);
        }
    }
}
