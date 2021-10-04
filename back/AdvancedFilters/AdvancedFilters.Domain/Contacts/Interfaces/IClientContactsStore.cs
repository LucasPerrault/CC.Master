using AdvancedFilters.Domain.Contacts.Filters;
using AdvancedFilters.Domain.Contacts.Models;
using Lucca.Core.Api.Abstractions.Paging;
using System.Threading.Tasks;

namespace AdvancedFilters.Domain.Contacts.Interfaces
{
    public interface IClientContactsStore
    {
        Task<Page<ClientContact>> GetAsync(IPageToken pageToken, ClientContactFilter filter);
    }
}