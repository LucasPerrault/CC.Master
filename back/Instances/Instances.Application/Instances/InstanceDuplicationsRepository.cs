using Instances.Domain.Instances;
using System;
using System.Threading.Tasks;

namespace Instances.Application.Instances
{
    public class InstanceDuplicationsRepository
    {
        private readonly IInstanceDuplicationsStore _duplicationsStore;

        public InstanceDuplicationsRepository(IInstanceDuplicationsStore duplicationsStore)
        {
            _duplicationsStore = duplicationsStore;
        }

        public Task<InstanceDuplication> GetDuplication(Guid id)
        {
            return _duplicationsStore.GetAsync(id);
        }
    }
}
