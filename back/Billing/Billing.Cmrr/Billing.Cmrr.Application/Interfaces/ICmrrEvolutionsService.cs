using Billing.Cmrr.Domain.Evolution;
using System.Threading.Tasks;

namespace Billing.Cmrr.Application.Interfaces
{
    public interface ICmrrEvolutionsService
    {
        Task<CmrrEvolution> GetEvolutionAsync(CmrrFilter evolutionFilter);
    }
}
