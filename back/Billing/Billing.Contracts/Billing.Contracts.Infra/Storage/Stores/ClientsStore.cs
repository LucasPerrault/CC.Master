using Billing.Contracts.Domain.Clients;
using Billing.Contracts.Domain.Clients.Interfaces;
using Billing.Contracts.Infra.Storage;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Billing.Contracts.Infra.Clients.Stores
{
    public class ClientsStore : IClientsStore
    {
        private readonly ContractsDbContext _dbContext;

        public ClientsStore(ContractsDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public Task<Client> GetByIdAsync(int id)
        {
            return _dbContext
                .FindAsync<Client>(id)
                .AsTask();
        }

        public Task<Client> GetByExternalIdAsync(Guid externalId)
        {
            var client = _dbContext.Set<Client>()
                .SingleOrDefault(c => c.ExternalId == externalId);
            return Task.FromResult(client);
        }
    }
}
