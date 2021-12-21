using AdvancedFilters.Domain.Core.Models;
using System;
using System.Collections.Generic;
using Tools;

namespace AdvancedFilters.Domain.Billing.Models
{
    public enum BillingEntity
    {
        Unknown = 0,
        France = 1,
        Iberia = 2,
    }
    public class Client : IDeepCopyable<Client>
    {
        public int Id { get; set; }
        public Guid ExternalId { get; set; }
        public string Name { get; set; }
        public BillingEntity BillingEntity { get; set; }

        public IEnumerable<Contract> Contracts { get; set; }

        public Client DeepCopy()
        {
            return this.DeepCopyByExpressionTree();
        }
    }
}
