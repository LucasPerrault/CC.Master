using AdvancedFilters.Domain.Core.Collections;
using AdvancedFilters.Domain.Filters.Builders;
using AdvancedFilters.Domain.Filters.Models;
using System;

namespace AdvancedFilters.Domain.Instance.Models
{
    public class AppInstance
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ApplicationName => ApplicationsCollection.GetName(ApplicationId) ?? Name;
        public string ApplicationId { get; set; }
        public int EnvironmentId { get; set; }
        public DateTime? DeletedAt { get; set; }

        public Environment Environment { get; set; }
    }

    public class AppInstanceAdvancedCriterion : AdvancedCriterion<AppInstance>
    {
        public SingleStringComparisonCriterion ApplicationId { get; set; }

        public override IQueryableExpressionBuilder<AppInstance> GetExpressionBuilder(IQueryableExpressionBuilderFactory factory)
            => factory.Create(this);
    }

    public class AppInstancesAdvancedCriterion : AppInstanceAdvancedCriterion, IListCriterion
    {
        public ItemsMatching ItemsMatched { get; set; }
    }
}
