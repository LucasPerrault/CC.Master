using Billing.Cmrr.Domain;
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
    }
}
