using AdvancedFilters.Domain.Instance.Models;
using AdvancedFilters.Infra.Filters.Builders.Chaining;
using System.Collections.Generic;
using System.Linq;
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
            yield return Apply(Criterion.CreatedAt).To(e => e.CreatedAt);
            yield return Apply(Criterion.Subdomain).To(e => e.Subdomain);
            yield return Apply(Criterion.Cluster).To(e => e.Cluster);
            yield return ApplyMany(Criterion.LegalUnits).To(e => e.LegalUnits);
            yield return ApplyMany(Criterion.AppInstances).To(
                e => e.AppInstances
                    .Where(i => i.ApplicationId != AppInstance.LuccaApplicationId)
            );
            yield return ApplyMany(Criterion.Distributors).To(e => e.Accesses.Select(a => a.Distributor).Distinct());
            yield return Apply(Criterion.DistributorType).To(EnvironmentExpressions.DistributorTypeFn);
        }


    }
}
