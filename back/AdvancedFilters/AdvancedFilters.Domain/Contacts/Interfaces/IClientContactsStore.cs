using AdvancedFilters.Domain.Contacts.Filters;
using AdvancedFilters.Domain.Contacts.Models;
using AdvancedFilters.Domain.Filters.Models;
using Lucca.Core.Api.Abstractions.Paging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdvancedFilters.Domain.Contacts.Interfaces
{
    public interface IClientContactsStore
    {
        Task<Page<ClientContact>> GetAsync(IPageToken pageToken, ClientContactFilter filter);
        Task<Page<ClientContact>> SearchAsync(IPageToken page, IAdvancedFilter criterion);
        Task<List<ClientContact>> SearchAsync(IAdvancedFilter criterion);
    }
}
