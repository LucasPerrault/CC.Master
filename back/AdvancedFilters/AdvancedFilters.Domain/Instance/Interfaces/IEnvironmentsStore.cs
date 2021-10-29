using AdvancedFilters.Domain.Filters.Models;
using AdvancedFilters.Domain.Instance.Filters;
using AdvancedFilters.Domain.Instance.Models;
using Lucca.Core.Api.Abstractions.Paging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdvancedFilters.Domain.Instance.Interfaces
{
    public interface IEnvironmentsStore
    {
        Task<Page<Environment>> GetAsync(IPageToken pageToken, EnvironmentFilter filter);
        Task<List<Environment>> GetAsync(EnvironmentFilter filter);
        Task<Page<Environment>> SearchAsync(IPageToken pageToken, IAdvancedFilter filter);
        Task<List<Environment>> SearchAsync(IAdvancedFilter filter);
        Task<List<string>> GetClustersAsync();
    }
}
