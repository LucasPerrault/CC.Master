using System;
using System.Threading.Tasks;

namespace Billing.Contracts.Domain.Clients.Interfaces
{
    public interface IClientsStore
    {
        Task<Client> GetByIdAsync(int id);
        Task<Client> GetByExternalIdAsync(Guid externalId);
    }
}
