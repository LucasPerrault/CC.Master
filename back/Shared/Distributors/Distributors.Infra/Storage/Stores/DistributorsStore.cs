using Distributors.Domain;
using Distributors.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Distributors.Infra.Storage.Stores
{
    public class DistributorsStore : IDistributorsStore
    {
        private readonly DistributorsDbContext _dbContext;

        public DistributorsStore(DistributorsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<Distributor> GetByIdAsync(int id)
        {
            return _dbContext.Set<Distributor>().FirstOrDefaultAsync(d => d.Id == id);
        }

        public Task<Distributor> GetByCodeAsync(string code)
        {
            return _dbContext.Set<Distributor>().FirstOrDefaultAsync(d => d.Code == code);
        }
    }
}
