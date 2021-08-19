using Lucca.Core.Api.Abstractions.Paging;
using Rights.Domain.Filtering;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Billing.Contracts.Domain.Clients.Interfaces
{
    public interface IClientsStore
    {
        Task<List<Client>> GetAsync(AccessRight accessRight, ClientFilter filter);
        Task<Page<Client>> GetPageAsync(IPageToken pageToken, AccessRight accessRight, ClientFilter filter);
    }
}
