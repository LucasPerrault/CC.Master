using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Instances.Domain.Shared
{
    public interface ICcDataService
    {
        Uri GetCcDataBaseUri(string cluster);
        Task StartDuplicateInstanceAsync(DuplicateInstanceRequestDto duplicateInstanceRequest, string cluster, string callbackPath);
        Task CreateInstanceBackupAsync(CreateInstanceBackupRequestDto createInstanceBackupRequest, string cluster);
        Task<bool> ResetInstanceCacheAsync(ResetInstanceCacheRequestDto resetInstanceCacheRequest, string cluster);
        Task DeleteInstanceAsync(string subdomain, string cluster, string callbackPath);
        Task DeleteInstancesAsync(IEnumerable<string> subdomains, string cluster, string callbackPath);
    }
}
