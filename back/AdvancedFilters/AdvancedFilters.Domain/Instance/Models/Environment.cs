using AdvancedFilters.Domain.Billing.Models;
using AdvancedFilters.Domain.Core.Models;
using AdvancedFilters.Domain.Filters.Builders;
using AdvancedFilters.Domain.Filters.Models;
using System;
using System.Collections.Generic;
using Tools;

namespace AdvancedFilters.Domain.Instance.Models
{
    public class Environment : IDeepCopyable<Environment>
    {
        public int Id { get; set; }
        public string Subdomain { get; set; }
        public string Domain { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Cluster { get; set; }
        public string ProductionHost { get; set; }

        public IEnumerable<LegalUnit> LegalUnits { get; set; }
        public IEnumerable<AppInstance> AppInstances { get; set; }
        public IEnumerable<Contract> Contracts { get; set; }
        public IEnumerable<EnvironmentAccess> Accesses { get; set; }

        public Environment DeepCopy()
        {
            return this.DeepCopyByExpressionTree();
        }
    }

    public class EnvironmentAdvancedCriterion : AdvancedCriterion<Environment>
    {
        public SingleDateTimeComparisonCriterion CreatedAt { get; set; }
        public SingleStringComparisonCriterion Subdomain { get; set; }
        public SingleStringComparisonCriterion Cluster { get; set; }
        public LegalUnitsAdvancedCriterion LegalUnits { get; set; }
        public AppInstancesAdvancedCriterion AppInstances { get; set; }
        public DistributorsAdvancedCriterion Distributors { get; set; }

        public override IQueryableExpressionBuilder<Environment> GetExpressionBuilder(IQueryableExpressionBuilderFactory factory)
            => factory.Create(this);
    }
}
