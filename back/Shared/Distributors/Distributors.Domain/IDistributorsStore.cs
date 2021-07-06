using Distributors.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Distributors.Domain
{
    public interface IDistributorsStore
    {
        Task<Distributor> GetByIdAsync(int id);
        Task<Distributor> GetByCodeAsync(string code);
        Task<List<Distributor>> GetAllAsync();
    }
}
