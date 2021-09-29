using AdvancedFilters.Domain.Billing.Models;
using AdvancedFilters.Domain.Core.Models;
using AdvancedFilters.Domain.Filters.Builders;
using AdvancedFilters.Domain.Filters.Models;
using System;
using System.Collections.Generic;

namespace AdvancedFilters.Domain.Instance.Models
{
    public class Environment
    {
        public int Id { get; set; }
        public string Subdomain { get; set; }
        public string Domain { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ProductionHost { get; set; }

        public IReadOnlyCollection<LegalUnit> LegalUnits { get; set; }
        public IReadOnlyCollection<AppInstance> AppInstances { get; set; }
        public IReadOnlyCollection<Contract> Contracts { get; set; }
    }

    public class EnvironmentAdvancedCriterion : AdvancedCriterion<Environment>
    {
        public SingleValueComparisonCriterion<string> Subdomain { get; set; }
        public LegalUnitAdvancedCriterion LegalUnits { get; set; }
        public AppInstanceAdvancedCriterion AppInstances { get; set; }

        public override IQueryableExpressionBuilder<Environment> GetExpressionBuilder(IQueryableExpressionBuilderFactory factory)
            => factory.Create(this);
    }
}
