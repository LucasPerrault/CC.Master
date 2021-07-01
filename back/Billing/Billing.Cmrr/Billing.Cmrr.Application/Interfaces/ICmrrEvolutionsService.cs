using Billing.Cmrr.Domain;
using Billing.Cmrr.Domain.Evolution;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Billing.Cmrr.Application.Interfaces
{
    public interface ICmrrEvolutionsService
    {
        Task<CmrrEvolution> GetEvolutionAsync(CmrrEvolutionFilter evolutionFilter);
    }

    public class CmrrEvolutionFilter
    {
        public DateTime StartPeriod { get; set; }
        public DateTime EndPeriod { get; set; }

        public HashSet<int> ClientId { get; set; } = new HashSet<int>();
        public HashSet<string> DistributorsId { get; set; } = new HashSet<string>();
        public HashSet<BillingStrategy> BillingStrategies { get; set; } = new HashSet<BillingStrategy>();
    }
}
