using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Instances.Domain.Demos
{

    public interface IDemoDuplicationsStore
    {
        Task<DemoDuplication> CreateAsync(DemoDuplication demo);
        IReadOnlyCollection<DemoDuplication> GetByIds(IReadOnlyCollection<int> ids);
        DemoDuplication GetByInstanceDuplicationId(Guid instanceDuplicationId);
    }
}
