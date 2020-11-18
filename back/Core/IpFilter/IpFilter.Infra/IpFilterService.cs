using IpFilter.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IpFilter.Infra
{
    public class IpFilterService : IIpFilterService
    {
        private readonly IIpFilterAuthorizationStore _store;

        public IpFilterService(IIpFilterAuthorizationStore store)
        {
            _store = store;
        }

        public async Task<IEnumerable<IpFilterAuthorization>> GetValid(IpFilterUser user)
        {
            var authorizations = await _store.GetByUserAsync(user);
            return authorizations.Where(IsCurrentlyValid);
        }

        public bool IsCurrentlyValid(IpFilterAuthorization ipFilterAuthorization)
        {
            return ipFilterAuthorization.CreatedAt < DateTime.Now
                && ipFilterAuthorization.ExpiresAt > DateTime.Now;
        }
    }
}
