using Environments.Domain;
using Environments.Domain.Storage;
using System.Threading.Tasks;

namespace Environments.Infra.Storage.Stores
{
    public class EnvironmentsRenamingStore : IEnvironmentsRenamingStore
    {
        private readonly EnvironmentsDbContext _dbContext;

        public EnvironmentsRenamingStore(EnvironmentsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<EnvironmentRenaming> CreateAsync(EnvironmentRenaming environmentRenaming)
        {
            var saveEnvironmentRenamingTracked = _dbContext.Add(environmentRenaming);
            await _dbContext.SaveChangesAsync();
            return saveEnvironmentRenamingTracked.Entity;
        }
    }
}
