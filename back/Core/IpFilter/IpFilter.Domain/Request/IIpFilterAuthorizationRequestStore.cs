using System.Collections.Generic;
using System.Threading.Tasks;

namespace IpFilter.Domain
{
    public interface IIpFilterAuthorizationRequestStore
    {
        public Task<List<IpFilterAuthorizationRequest>> GetAsync(IpFilterAuthorizationRequestFilter filter);
        public Task<IpFilterAuthorizationRequest> FirstOrDefaultAsync(IpFilterAuthorizationRequestFilter filter);
        public Task<IpFilterAuthorizationRequest> CreateAsync(IpFilterAuthorizationRequest authorizationRequest);
        Task RevokeAsync(IpFilterAuthorizationRequest validRequest);
    }
}
