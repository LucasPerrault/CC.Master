using Lucca.Core.Api.Abstractions.Paging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdvancedFilters.Domain.DataSources
{
    public interface ISyncService
    {
        Task SyncEverythingAsync();
        Task SyncMultiTenantDataAsync();
        Task SyncMonoTenantDataAsync(HashSet<string> subdomains);
        Task SyncRandomMonoTenantDataAsync(int tenantCount);
        Task SyncRangeMonoTenantDataAsync(IPageToken pageToken);
        Task PurgeEverythingAsync();
        Task PurgeTenantsAsync(HashSet<string> subdomains);
    }
}
