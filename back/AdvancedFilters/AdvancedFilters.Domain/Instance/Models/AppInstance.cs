using AdvancedFilters.Domain.Filters.Models;
using System;

namespace AdvancedFilters.Domain.Instance.Models
{
    public class AppInstance
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ApplicationId { get; set; }
        public int EnvironmentId { get; set; }
        public DateTime? DeletedAt { get; set; }

        public Environment Environment { get; set; }
    }

    public class AppInstanceAdvancedCriterion : AdvancedCriterion
    {
        public SingleValueComparisonCriterion<string> ApplicationId { get; set; }
    }
}
