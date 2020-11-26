using System.Collections.Generic;
using System.Threading.Tasks;

namespace IpFilter.Domain
{
    public interface IIpFilterService
    {
        Task<IEnumerable<IpFilterAuthorization>> GetValidAsync(IpFilterUser user);
        bool IsCurrentlyValid(IpFilterAuthorization ipFilterAuthorization);
    }
}
