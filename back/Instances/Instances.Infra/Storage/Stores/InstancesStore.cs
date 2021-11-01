using Instances.Domain.Instances;
using Instances.Domain.Instances.Models;
using Instances.Infra.Instances.Services;
using Lucca.Core.Api.Queryable.Paging;
using Lucca.Core.Shared.Domain.Exceptions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Instances.Infra.Storage.Stores
{
    public class InstancesStore : IInstancesStore
    {

        private readonly InstancesDbContext _dbContext;
        private readonly IQueryPager _queryPager;
        private readonly IInstancesRemoteStore _remoteStore;

        public InstancesStore(InstancesDbContext dbContext, IQueryPager queryPager, IInstancesRemoteStore remoteStore)
        {
            _dbContext = dbContext;
            _queryPager = queryPager;
            _remoteStore = remoteStore;
        }

        public Task<Instance> CreateForTrainingAsync(int environmentId, bool isAnonymized)
        {
            return _remoteStore.CreateForTrainingAsync(environmentId, isAnonymized);
        }

        public Task<Instance> CreateForDemoAsync(string password)
        {
            return _remoteStore.CreateForDemoAsync(password);
        }

        public Task DeleteByIdAsync(int instanceId)
        {
            return _remoteStore.DeleteByIdAsync(instanceId);
        }

        public Task DeleteByIdsAsync(IEnumerable<int> instanceIds)
        {
            return _remoteStore.DeleteByIdsAsync(instanceIds);
        }

        private IQueryable<Instance> Instances => _dbContext.Set<Instance>();
    }
}
