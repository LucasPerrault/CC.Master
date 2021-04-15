using Lucca.Core.Api.Abstractions.Paging;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Instances.Domain.Demos
{

    public interface IDemoDuplicationsStore
    {
        Task<DemoDuplication> CreateAsync(DemoDuplication demo);
        DemoDuplication GetByInstanceDuplicationId(Guid instanceDuplicationId);
        Task UpdateProgressAsync(DemoDuplication duplication, DemoDuplicationProgress progress);
    }
}
