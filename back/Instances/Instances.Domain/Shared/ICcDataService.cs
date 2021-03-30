using System;
using System.Threading.Tasks;

namespace Instances.Domain.Shared
{
    public interface ICcDataService
    {
        Uri GetCcDataBaseUri(string cluster);
        Task StartDuplicateInstanceAsync(DuplicateInstanceRequestDto duplicateInstanceRequest, string cluster, string callbackPath);
    }
}
