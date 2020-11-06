using System.Collections.Generic;
using System.Threading.Tasks;

namespace IpFilter.Domain
{
    public interface IIpFilterAuthorizationStore
    {
        Task<IReadOnlyCollection<IpFilterAuthorization>> GetByUserAsync(IpFilterUser user);
    }
}
