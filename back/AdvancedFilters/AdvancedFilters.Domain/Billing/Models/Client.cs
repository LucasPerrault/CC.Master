using AdvancedFilters.Domain.Core.Models;
using System;
using System.Collections.Generic;
using Tools;

namespace AdvancedFilters.Domain.Billing.Models
{
    public class Client : IDeepCopyable<Client>
    {
        public int Id { get; set; }
        public Guid ExternalId { get; set; }
        public string Name { get; set; }

        public IReadOnlyCollection<Contract> Contracts { get; set; }

        public Client DeepCopy()
        {
            return this.DeepCopyByExpressionTree();
        }
    }
}
