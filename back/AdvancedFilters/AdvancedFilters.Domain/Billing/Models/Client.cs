using System;
using System.Collections.Generic;
using AdvancedFilters.Domain.Filters.Builders;
using AdvancedFilters.Domain.Filters.Models;

namespace AdvancedFilters.Domain.Billing.Models
{
    public enum BillingEntity
    {
        Unknown = 0,
        France = 1,
        Iberia = 2,
        Switzerland = 3,
    }

    public class Client
    {
        public int Id { get; set; }
        public Guid ExternalId { get; set; }
        public string Name { get; set; }
        public BillingEntity BillingEntity { get; set; }

        public IEnumerable<Contract> Contracts { get; set; }
    }

    public class ClientAdvancedCriterion : AdvancedCriterion<Client>
    {
        public SingleEnumComparisonCriterion<BillingEntity> BillingEntity { get; set; }

        public override IQueryableExpressionBuilder<Client> GetExpressionBuilder(IQueryableExpressionBuilderFactory factory)
            => factory.Create(this);
    }

    public class ClientsAdvancedCriterion : ClientAdvancedCriterion, IListCriterion
    {
        public ItemsMatching ItemsMatched { get; set; }
    }
}
