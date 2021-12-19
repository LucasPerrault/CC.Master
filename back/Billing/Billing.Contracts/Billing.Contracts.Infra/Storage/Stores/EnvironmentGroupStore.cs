using Billing.Contracts.Domain.Contracts;
using Billing.Contracts.Domain.Contracts.Interfaces;
using Billing.Contracts.Domain.Counts;
using Rights.Domain.Filtering;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Billing.Contracts.Infra.Storage.Stores
{
    public class EnvironmentGroupStore : IEnvironmentGroupStore
    {
        private readonly IContractsStore _store;

        public EnvironmentGroupStore(IContractsStore store)
        {
            _store = store;
        }

        public async Task<List<EnvironmentWithContractGroup>> GetEnvGroupsAsync(IEnumerable<int> envGroupIds)
        {
            var contracts = await _store.GetAsync(AccessRight.All, new ContractFilter());

            return contracts
                .GroupBy(c => c.Environment.GroupId)
                .Where(g => envGroupIds.Contains(g.Key))
                .Select
                (
                    g => new EnvironmentWithContractGroup
                    {
                        EnvironmentGroupId = g.Key,
                        Contracts = g.ToList(),
                    }
                ).ToList();
        }
    }
}
