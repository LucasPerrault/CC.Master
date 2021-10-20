using Billing.Contracts.Domain.Environments;
using Rights.Domain.Filtering;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Billing.Contracts.Domain.Clients.Interfaces
{
    public interface IContractEnvironmentStore
    {
        Task<List<ContractEnvironment>> GetAsync(AccessRight accessRight, HashSet<int> environmentIds);
    }
}
