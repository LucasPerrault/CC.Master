using Billing.Cmrr.Application.Interfaces;
using Billing.Cmrr.Domain;
using System;
using System.Collections.Generic;

namespace Billing.Web
{
    public class CmrrEvolutionListQuery
    {
        public DateTime? StartPeriod { get; set; }
        public DateTime? EndPeriod { get; set; }

        public HashSet<BillingStrategy> BillingStrategy { get; set; } = new HashSet<BillingStrategy>();
        public HashSet<int> ClientId { get; set; } = new HashSet<int>();
        public HashSet<string> DistributorId { get; set; } = new HashSet<string>();

        public CmrrEvolutionFilter ToCmrrEvolutionFilter()
        {
            if (StartPeriod is null)
                throw new ArgumentNullException($"{nameof(StartPeriod)} must be specified");
            if (EndPeriod is null)
                throw new ArgumentNullException($"{nameof(EndPeriod)} must be specified");

            return new CmrrEvolutionFilter
            {
                StartPeriod = StartPeriod.Value,
                EndPeriod = EndPeriod.Value,
                ClientId = ClientId,
                DistributorsId = DistributorId,
                BillingStrategies = BillingStrategy
            };
        }
    }
}
