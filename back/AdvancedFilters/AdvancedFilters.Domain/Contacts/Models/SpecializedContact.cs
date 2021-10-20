using AdvancedFilters.Domain.Core.Models;
using AdvancedFilters.Domain.Filters.Builders;
using AdvancedFilters.Domain.Filters.Models;
using AdvancedFilters.Domain.Instance.Models;
using System;
using Tools;
using Environment = AdvancedFilters.Domain.Instance.Models.Environment;

namespace AdvancedFilters.Domain.Contacts.Models
{
    public class SpecializedContact : IDeepCopyable<SpecializedContact>
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public string RoleCode { get; set; }
        public int UserId { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string UserMail { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public bool IsConfirmed { get; set; }
        public int EnvironmentId { get; set; }
        public int EstablishmentId { get; set; }

        public Environment Environment { get; set; }
        public Establishment Establishment { get; set; }

        public SpecializedContact DeepCopy()
        {
            return this.DeepCopyByExpressionTree();
        }
    }

    public class SpecializedContactAdvancedCriterion : AdvancedCriterion<SpecializedContact>
    {
        public SingleBooleanComparisonCriterion IsConfirmed { get; set; }
        public SingleStringComparisonCriterion RoleCode { get; set; }
        public EnvironmentAdvancedCriterion Environment { get; set; }
        public LegalUnitAdvancedCriterion LegalUnit { get; set; }

        public override IQueryableExpressionBuilder<SpecializedContact> GetExpressionBuilder(IQueryableExpressionBuilderFactory factory)
            => factory.Create(this);
    }
}
