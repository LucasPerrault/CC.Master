using Instances.Domain.Demos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Instances.Infra.Storage.Stores
{
    public class DemoDuplicationsStore : IDemoDuplicationsStore
    {
        private readonly InstancesDbContext _dbContext;

        public DemoDuplicationsStore(InstancesDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<DemoDuplication> CreateAsync(DemoDuplication duplication)
        {
            await _dbContext.Set<DemoDuplication>().AddAsync(duplication);
            await _dbContext.SaveChangesAsync();
            return duplication;
        }

        public DemoDuplication GetByInstanceDuplicationId(Guid instanceDuplicationId)
        {
            return Duplications.Single(d => d.InstanceDuplicationId == instanceDuplicationId);
        }

        public async Task UpdateProgressAsync(DemoDuplication duplication, DemoDuplicationProgress progress)
        {
            duplication.Progress = progress;
            await _dbContext.SaveChangesAsync();
        }

        private IQueryable<DemoDuplication> Duplications => _dbContext.Set<DemoDuplication>()
            .Include(d => d.InstanceDuplication.Distributor);
    }
}
