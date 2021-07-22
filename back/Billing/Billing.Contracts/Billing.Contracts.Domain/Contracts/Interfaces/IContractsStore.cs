using Rights.Domain.Filtering;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Billing.Contracts.Domain.Contracts.Interfaces
{
    public interface IContractsStore
    {
        Task<List<Contract>> GetAsync(AccessRight accessRight, ContractFilter filter);
    }
}
