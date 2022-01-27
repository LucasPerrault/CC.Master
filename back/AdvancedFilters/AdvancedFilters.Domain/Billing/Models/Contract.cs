using System;
using System.Collections.Generic;
using AdvancedFilters.Domain.Filters.Builders;
using AdvancedFilters.Domain.Filters.Models;
using Environment = AdvancedFilters.Domain.Instance.Models.Environment;

namespace AdvancedFilters.Domain.Billing.Models
{
    public class Contract
    {
        public int Id { get; set; }
        public Guid ExternalId { get; set; }
        public int ClientId { get; set; }
        public int EnvironmentId { get; set; }

        public Client Client { get; set; }
        
        public Environment Environment { get; set; }
    }

    public class ContractAdvancedCriterion : AdvancedCriterion<Contract>
    {
        public ClientAdvancedCriterion Client { get; set; }

        public override IQueryableExpressionBuilder<Contract> GetExpressionBuilder(IQueryableExpressionBuilderFactory factory)
            => factory.Create(this);
    }

    public class ContractsAdvancedCriterion : ContractAdvancedCriterion, IListCriterion
    {
        public ItemsMatching ItemsMatched { get; set; }
    }
}
