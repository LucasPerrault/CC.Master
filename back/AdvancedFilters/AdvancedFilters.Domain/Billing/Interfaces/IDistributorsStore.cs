using AdvancedFilters.Domain.Billing.Filters;
using AdvancedFilters.Domain.Billing.Models;
using Lucca.Core.Api.Abstractions.Paging;
using System.Threading.Tasks;

namespace AdvancedFilters.Domain.Billing.Interfaces
{
    public interface IDistributorsStore
    {
        Task<Page<Distributor>> GetAsync(IPageToken pageToken, DistributorFilter filter);
    }
}
