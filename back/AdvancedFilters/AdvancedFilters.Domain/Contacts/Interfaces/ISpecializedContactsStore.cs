using AdvancedFilters.Domain.Contacts.Filters;
using AdvancedFilters.Domain.Contacts.Models;
using Lucca.Core.Api.Abstractions.Paging;
using System.Threading.Tasks;

namespace AdvancedFilters.Domain.Contacts.Interfaces
{
    public interface ISpecializedContactsStore
    {
        Task<Page<SpecializedContact>> GetAsync(IPageToken pageToken, SpecializedContactFilter filter);
    }
}
