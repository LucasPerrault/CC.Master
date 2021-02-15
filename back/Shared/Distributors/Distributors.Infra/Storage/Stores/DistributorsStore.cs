using Distributors.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Distributors.Infra.Storage.Stores
{
    public class DistributorsStore
    {
        private readonly DistributorsDbContext _dbContext;

        public DistributorsStore(DistributorsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<Distributor> GetByIdAsync(string id)
        {
            return _dbContext.Set<Distributor>().FirstOrDefaultAsync(d => d.Id == id);
        }

        public Task<Distributor> GetByCodeAsync(string code)
        {
            return _dbContext.Set<Distributor>().FirstOrDefaultAsync(d => d.Code == code);
        }
    }
}
