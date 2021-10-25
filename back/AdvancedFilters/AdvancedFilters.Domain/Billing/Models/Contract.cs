using AdvancedFilters.Domain.Core.Models;
using System;
using System.Collections.Generic;
using Tools;
using Environment = AdvancedFilters.Domain.Instance.Models.Environment;

namespace AdvancedFilters.Domain.Billing.Models
{
    public class Contract : IDeepCopyable<Contract>
    {
        public int Id { get; set; }
        public Guid ExternalId { get; set; }
        public int ClientId { get; set; }
        public int EnvironmentId { get; set; }

        public Client Client { get; set; }
        public IEnumerable<EstablishmentContract> EstablishmentAttachments { get; set; }

        public Environment Environment { get; set; }
        
        public Contract DeepCopy()
        {
            return this.DeepCopyByExpressionTree();
        }
    }
}
