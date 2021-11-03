using AdvancedFilters.Domain.Contacts.Filters;
using AdvancedFilters.Domain.Contacts.Models;
using AdvancedFilters.Domain.Filters.Models;
using Lucca.Core.Api.Abstractions.Paging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdvancedFilters.Domain.Contacts.Interfaces
{
    public interface ISpecializedContactsStore
    {
        Task<Page<SpecializedContact>> GetAsync(IPageToken pageToken, SpecializedContactFilter filter);
        Task<Page<SpecializedContact>> SearchAsync(IPageToken page, IAdvancedFilter criterion);
        Task<List<SpecializedContact>> SearchAsync(IAdvancedFilter criterion);
        Task<Page<string>> GetAllRoleCodesAsync();
    }
}
