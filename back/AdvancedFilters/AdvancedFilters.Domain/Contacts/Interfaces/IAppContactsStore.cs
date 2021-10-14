using AdvancedFilters.Domain.Contacts.Filters;
using AdvancedFilters.Domain.Contacts.Models;
using AdvancedFilters.Domain.Filters.Models;
using Lucca.Core.Api.Abstractions.Paging;
using System.Threading.Tasks;

namespace AdvancedFilters.Domain.Contacts.Interfaces
{
    public interface IAppContactsStore
    {
        Task<Page<AppContact>> GetAsync(IPageToken pageToken, AppContactFilter filter);
        Task<Page<AppContact>> SearchAsync(IPageToken page, IAdvancedFilter criterion);
    }
}
