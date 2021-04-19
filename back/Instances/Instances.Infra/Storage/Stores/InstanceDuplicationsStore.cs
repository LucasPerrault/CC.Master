using Instances.Domain.Instances;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Instances.Infra.Storage.Stores
{
    public class InstanceDuplicationsStore : IInstanceDuplicationsStore
    {
        private readonly InstancesDbContext _dbContext;

        public InstanceDuplicationsStore(InstancesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<InstanceDuplication> GetAsync(Guid id)
        {
            return _dbContext.Set<InstanceDuplication>()
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task UpdateProgressAsync(InstanceDuplication duplication, InstanceDuplicationProgress progress)
        {
            duplication.Progress = progress;
            await _dbContext.SaveChangesAsync();
        }
    }
}
