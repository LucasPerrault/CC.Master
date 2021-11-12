using Distributors.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Distributors.Domain
{
    public interface IDistributorsStore
    {
        Task<Distributor> GetActiveByIdAsync(int id);
        Task<Distributor> GetActiveByCodeAsync(string code);
        Task<List<Distributor>> GetAllAsync();
    }
}
