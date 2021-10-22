using Environments.Domain;
using Environments.Domain.Storage;
using System.Threading.Tasks;

namespace Environments.Infra.Storage.Stores
{
    public class EnvironmentLogsStore : IEnvironmentLogsStore
    {
        private readonly EnvironmentsDbContext _dbContext;

        public EnvironmentLogsStore(EnvironmentsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<EnvironmentLog> CreateAsync(EnvironmentLog log)
        {
            _dbContext.Add(log);
            await _dbContext.SaveChangesAsync();
            return log;
        }
    }
}
