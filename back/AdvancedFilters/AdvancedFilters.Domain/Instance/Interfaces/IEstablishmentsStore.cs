using System.Collections.Generic;
using AdvancedFilters.Domain.Instance.Filters;
using AdvancedFilters.Domain.Instance.Models;
using Lucca.Core.Api.Abstractions.Paging;
using System.Threading.Tasks;
using AdvancedFilters.Domain.Filters.Models;

namespace AdvancedFilters.Domain.Instance.Interfaces
{
    public interface IEstablishmentsStore
    {
        Task<Page<Establishment>> GetAsync(IPageToken pageToken, EstablishmentFilter filter);
        Task<Page<Establishment>> SearchAsync(IPageToken pageToken, IAdvancedFilter criterion);
        Task<List<Establishment>> SearchAsync(IAdvancedFilter filter);
    }
}
