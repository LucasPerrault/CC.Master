using Instances.Domain.Trainings;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Instances.Infra.Storage.Stores
{
    public class TrainingRestorationsStore : ITrainingRestorationsStore
    {
        private readonly InstancesDbContext _dbContext;

        public TrainingRestorationsStore(InstancesDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<TrainingRestoration> CreateAsync(TrainingRestoration restoration)
        {
            _dbContext.Set<TrainingRestoration>().Add(restoration);
            await _dbContext.SaveChangesAsync();
            return restoration;
        }

        public async Task<IReadOnlyCollection<TrainingRestoration>> GetByIdsAsync(IReadOnlyCollection<int> ids)
        {
            return await _dbContext.Set<TrainingRestoration>()
                .Where(d => ids.Contains(d.Id))
                .ToListAsync();
        }

        public Task<TrainingRestoration> GetByInstanceDuplicationIdAsync(Guid instanceDuplicationId)
        {
            return Duplications.SingleOrDefaultAsync(d => d.InstanceDuplicationId == instanceDuplicationId);
        }

        private IQueryable<TrainingRestoration> Duplications => _dbContext.Set<TrainingRestoration>()
            .Include(d => d.Environment)
            .Include(d => d.InstanceDuplication.Distributor);
    }
}
