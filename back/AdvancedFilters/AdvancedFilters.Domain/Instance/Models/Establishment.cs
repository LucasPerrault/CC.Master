using System;
using AdvancedFilters.Domain.Filters.Builders;
using AdvancedFilters.Domain.Filters.Models;

namespace AdvancedFilters.Domain.Instance.Models
{
    public class Establishment
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int LegalUnitId { get; set; }
        public string LegalIdentificationNumber { get; set; }
        public string ActivityCode { get; set; }
        public string TimeZoneId { get; set; }
        public int UsersCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsArchived { get; set; }
        public int EnvironmentId { get; set; }

        public LegalUnit LegalUnit { get; set; }
        public Environment Environment { get; set; }
    }

    public class EstablishmentAdvancedCriterion : AdvancedCriterion<Establishment>
    {
        public EnvironmentAdvancedCriterion Environment { get; set; }
        public LegalUnitAdvancedCriterion LegalUnit { get; set; }

        public override IQueryableExpressionBuilder<Establishment> GetExpressionBuilder(IQueryableExpressionBuilderFactory factory)
            => factory.Create(this);
    }
}
