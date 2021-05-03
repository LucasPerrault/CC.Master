using System;
using System.Threading.Tasks;

namespace Instances.Domain.Instances
{
    public interface IInstanceDuplicationsStore
    {
        Task<InstanceDuplication> GetAsync(Guid id);
        Task MarkAsCompleteAsync(InstanceDuplication duplication, InstanceDuplicationProgress progress);
    }
}
