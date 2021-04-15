using Distributors.Domain.Models;
using System.Threading.Tasks;

namespace Distributors.Domain
{
    public interface IDistributorsStore
    {
        Task<Distributor> GetByIdAsync(string id);
        Task<Distributor> GetByCodeAsync(string code);
    }
}
