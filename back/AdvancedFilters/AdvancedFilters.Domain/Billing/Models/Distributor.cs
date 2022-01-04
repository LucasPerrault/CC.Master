using AdvancedFilters.Domain.Filters.Builders;
using AdvancedFilters.Domain.Filters.Models;
using AdvancedFilters.Domain.Instance.Models;
using System.Collections.Generic;

namespace AdvancedFilters.Domain.Billing.Models
{
    public class Distributor
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int DepartmentId { get; set; }
        public bool IsAllowingCommercialCommunication { get; set; }

        public IEnumerable<EnvironmentAccess> EnvironmentAccesses { get; set; }
    }

    public class DistributorAdvancedCriterion : AdvancedCriterion<Distributor>
    {
        public SingleIntComparisonCriterion Id { get; set; }

        public override IQueryableExpressionBuilder<Distributor> GetExpressionBuilder(IQueryableExpressionBuilderFactory factory)
            => factory.Create(this);
    }

    public class DistributorsAdvancedCriterion : DistributorAdvancedCriterion, IListCriterion
    {
        public ItemsMatching ItemsMatched { get; set; }
    }
}
