using System;
using System.Threading.Tasks;

namespace Instances.Domain.Instances
{
    public interface IInstanceDuplicationsStore
    {
        Task<InstanceDuplication> GetAsync(Guid id);
        Task UpdateProgressAsync(InstanceDuplication duplication, InstanceDuplicationProgress progress);
    }
}
