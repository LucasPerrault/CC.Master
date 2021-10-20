using Billing.Contracts.Application.Clients;
using Billing.Contracts.Domain.Clients.Interfaces;
using Billing.Contracts.Domain.Contracts;
using Billing.Contracts.Domain.Contracts.Health;
using Billing.Contracts.Domain.Environments;
using Rights.Domain.Filtering;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Billing.Contracts.Application
{

    public class ContractHealthHelper
    {
        private readonly ContractsRepository _contractsRepository;
        private readonly IContractEnvironmentStore _contractEnvironmentStore;
        private readonly ContractHealthService _contractHealthService;

        public ContractHealthHelper(ContractsRepository contractsRepository, IContractEnvironmentStore contractEnvironmentStore, ContractHealthService contractHealthService)
        {
            _contractsRepository = contractsRepository;
            _contractEnvironmentStore = contractEnvironmentStore;
            _contractHealthService = contractHealthService;
        }

        public async Task<List<ContractHealth>> GetHealthAsync(HashSet<int> environmentIds)
        {
            var environmentWithContracts = await GetKnownEnvironmentsWithContractsAsync(environmentIds);
            return environmentWithContracts
                .SelectMany(ewc => _contractHealthService.GetHealth(ewc.Environment, ewc.Contracts))
                .ToList();
        }

        private async Task<List<EnvironmentWithContracts>> GetKnownEnvironmentsWithContractsAsync(HashSet<int> environmentIds)
        {
            var contractsFilter = new ContractFilter
            {
                EnvironmentIds = environmentIds
            };
            var contracts = await _contractsRepository.GetAsync(contractsFilter);
            var contractsPerEnvironment = contracts
                .Where(c => c.EnvironmentId.HasValue)
                .GroupBy(c => c.EnvironmentId.Value)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.ToList());

            var environments = await _contractEnvironmentStore.GetAsync(AccessRight.All, contractsPerEnvironment.Keys.ToHashSet());

            return environments.Select
            (
                e => new EnvironmentWithContracts
                {
                    Environment = e,
                    Contracts = contractsPerEnvironment[e.Id]
                }
            ).ToList();
        }

        private class EnvironmentWithContracts
        {
            public ContractEnvironment Environment { get; set; }
            public List<Contract> Contracts { get; set; }
        }
    }
}
