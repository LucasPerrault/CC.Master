using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Instances.Domain.Instances
{
    public interface IInstanceDuplicationsStore
    {
        Task<InstanceDuplication> GetAsync(Guid id);
        Task MarkAsCompleteAsync(InstanceDuplication duplication, InstanceDuplicationProgress progress);
        Task<IReadOnlyCollection<InstanceDuplication>> GetPendingForSubdomainAsync(string subdomain);
    }
}
