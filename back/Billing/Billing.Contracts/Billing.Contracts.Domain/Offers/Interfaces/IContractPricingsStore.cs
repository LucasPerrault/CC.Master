using System.Collections.Generic;
using System.Threading.Tasks;

namespace Billing.Contracts.Domain.Offers.Interfaces
{
    public interface IContractPricingsStore
    {
        Task<List<ContractPricing>> GetAsync();
    }
}
