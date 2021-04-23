using Instances.Domain.Instances;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Tools;

namespace Instances.Infra.Storage.Stores
{
    public class InstanceDuplicationsStore : IInstanceDuplicationsStore
    {
        private readonly InstancesDbContext _dbContext;
        private readonly ITimeProvider _timeProvider;

        public InstanceDuplicationsStore(InstancesDbContext dbContext, ITimeProvider timeProvider)
        {
            _dbContext = dbContext;
            _timeProvider = timeProvider;
        }

        public Task<InstanceDuplication> GetAsync(Guid id)
        {
            return _dbContext.Set<InstanceDuplication>()
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task MarkAsCompleteAsync(InstanceDuplication duplication, InstanceDuplicationProgress progress)
        {
            duplication.Progress = progress;
            duplication.EndedAt = _timeProvider.Now();
            await _dbContext.SaveChangesAsync();
        }
    }
}
