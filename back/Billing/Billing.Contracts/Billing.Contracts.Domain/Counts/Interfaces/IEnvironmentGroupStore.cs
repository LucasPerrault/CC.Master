using System.Collections.Generic;
using System.Threading.Tasks;

namespace Billing.Contracts.Domain.Counts
{
    public interface IEnvironmentGroupStore
    {
        Task<List<EnvironmentWithContractGroup>> GetEnvGroupsAsync(IEnumerable<int> envGroupIds);
    }
}
