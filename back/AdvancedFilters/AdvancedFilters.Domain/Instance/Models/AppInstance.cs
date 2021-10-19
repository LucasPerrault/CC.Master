using AdvancedFilters.Domain.Core.Models;
using AdvancedFilters.Domain.Filters.Builders;
using AdvancedFilters.Domain.Filters.Models;
using System;
using Tools;

namespace AdvancedFilters.Domain.Instance.Models
{
    public class AppInstance : IDeepCopyable<AppInstance>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ApplicationId { get; set; }
        public int EnvironmentId { get; set; }
        public DateTime? DeletedAt { get; set; }

        public Environment Environment { get; set; }

        public AppInstance DeepCopy()
        {
            return this.DeepCopyByExpressionTree();
        }
    }

    public class AppInstanceAdvancedCriterion : AdvancedCriterion<AppInstance>
    {
        public SingleStringComparisonCriterion ApplicationId { get; set; }

        public override IQueryableExpressionBuilder<AppInstance> GetExpressionBuilder(IQueryableExpressionBuilderFactory factory)
            => factory.Create(this);
    }
}
