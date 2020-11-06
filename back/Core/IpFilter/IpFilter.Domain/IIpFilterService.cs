using System.Threading.Tasks;

namespace IpFilter.Domain
{
    public interface IIpFilterService
    {
        Task<bool> HasCurrentlyValidAccess(IpFilterUser user);
        bool IsCurrentlyValid(IpFilterAuthorization ipFilterAuthorization);
    }
}
