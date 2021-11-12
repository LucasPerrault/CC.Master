using AdvancedFilters.Domain.Billing.Models;
using AdvancedFilters.Domain.Core.Models;
using AdvancedFilters.Domain.Filters.Builders;
using AdvancedFilters.Domain.Filters.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Tools;

namespace AdvancedFilters.Domain.Instance.Models
{
    public class Environment : IDeepCopyable<Environment>
    {
        public const int LuccaDistributorId = 37;

        public int Id { get; set; }
        public string Subdomain { get; set; }
        public string Domain { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Cluster { get; set; }
        public string ProductionHost { get; set; }

        public DistributorType DistributorType => EnvironmentExpressions.CompiledDistributorTypeFn(this);

        public IEnumerable<LegalUnit> LegalUnits { get; set; }
        public IEnumerable<AppInstance> AppInstances { get; set; }
        public IEnumerable<Contract> Contracts { get; set; }
        public IEnumerable<EnvironmentAccess> Accesses { get; set; } = new List<EnvironmentAccess>();

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
        public SingleEnumComparisonCriterion<DistributorType> DistributorType { get; set; }

        public override IQueryableExpressionBuilder<Environment> GetExpressionBuilder(IQueryableExpressionBuilderFactory factory)
            => factory.Create(this);
    }

    public enum DistributorType
    {
        Direct,
        Indirect
    }

    public static class EnvironmentExpressions
    {
        public static Expression<Func<Environment, DistributorType>> DistributorTypeFn
            => e => e.Accesses
                .Where(a => a.Type == EnvironmentAccessType.Contract)
                .All(a => a.DistributorId == Environment.LuccaDistributorId)
                ? DistributorType.Direct
                : DistributorType.Indirect;

        public static Func<Environment, DistributorType> CompiledDistributorTypeFn
            => DistributorTypeFn.Compile();
    }
}
