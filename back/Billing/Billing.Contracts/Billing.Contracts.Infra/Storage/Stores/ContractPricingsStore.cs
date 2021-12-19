using Billing.Contracts.Domain.Offers;
using Billing.Contracts.Domain.Offers.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Billing.Contracts.Infra.Storage.Stores
{
    public class ContractPricingsStore : IContractPricingsStore
    {
        private readonly ContractsDbContext _dbContext;

        private List<ContractPricing> _pricings;

        public ContractPricingsStore(ContractsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<ContractPricing>> GetAsync()
        {
            return _pricings ??= await _dbContext.Set<ContractPricing>().ToListAsync();
        }
    }
}
