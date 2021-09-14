using AdvancedFilters.Domain.Instance.Filters;
using AdvancedFilters.Domain.Instance.Models;
using Lucca.Core.Api.Abstractions.Paging;
using System.Threading.Tasks;

namespace AdvancedFilters.Domain.Instance.Interfaces
{
    public interface IEstablishmentsStore
    {
        Task<Page<Establishment>> GetAsync(IPageToken pageToken, EstablishmentFilter filter);
    }
}
