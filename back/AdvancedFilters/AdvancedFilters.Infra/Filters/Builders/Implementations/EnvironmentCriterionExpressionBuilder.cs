using AdvancedFilters.Domain.Instance.Models;
using AdvancedFilters.Infra.Filters.Builders.Chaining;
using System.Collections.Generic;
using Environment = AdvancedFilters.Domain.Instance.Models.Environment;

namespace AdvancedFilters.Infra.Filters.Builders.Implementations
{
    internal class EnvironmentCriterionExpressionBuilder : AdvancedCriterionExpressionBuilder<Environment, EnvironmentAdvancedCriterion>
    {
        public EnvironmentCriterionExpressionBuilder(EnvironmentAdvancedCriterion criterion)
            : base(criterion)
        { }

        protected override IEnumerable<IPropertyExpressionBuilder<Environment>> GetCriteria()
        {
            yield return Apply(Criterion.Subdomain).To(e => e.Subdomain);
            yield return Apply(Criterion.LegalUnits).To(e => e.LegalUnits);
            yield return Apply(Criterion.AppInstances).To(e => e.AppInstances);
        }
    }
}
