using Instances.Domain.Demos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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
            _dbContext.Set<DemoDuplication>().Add(duplication);
            await _dbContext.SaveChangesAsync();
            return duplication;
        }

        public IReadOnlyCollection<DemoDuplication> GetByIds(IReadOnlyCollection<int> ids)
        {
            return _dbContext.Set<DemoDuplication>().Where(d => ids.Contains(d.Id)).ToList();
        }

        public DemoDuplication GetByInstanceDuplicationId(Guid instanceDuplicationId)
        {
            return Duplications.Single(d => d.InstanceDuplicationId == instanceDuplicationId);
        }

        private IQueryable<DemoDuplication> Duplications => _dbContext.Set<DemoDuplication>()
            .Include(d => d.InstanceDuplication.Distributor);
    }
}
