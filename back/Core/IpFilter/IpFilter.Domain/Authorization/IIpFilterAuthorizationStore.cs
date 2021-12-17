using System.Collections.Generic;
using System.Threading.Tasks;

namespace IpFilter.Domain
{
    public interface IIpFilterAuthorizationStore
    {
        Task<IReadOnlyCollection<IpFilterAuthorization>> GetAsync(IpFilterAuthorizationFilter filter);
        Task<IpFilterAuthorization> CreateAsync(IpFilterAuthorization authorization);
        Task<bool> ExistsAsync(int requestId);
    }
}
