using System.Threading.Tasks;

namespace Billing.Contracts.Domain.Clients.Interfaces
{
    public interface ILegacyClientsRemoteService
    {
        Task SyncAsync();
    }
}
