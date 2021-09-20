using Authentication.Domain;
using Distributors.Domain;
using Distributors.Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Distributors.Application
{
    public class DistributorsRepository
    {
        private readonly IDistributorsStore _store;
        private readonly ClaimsPrincipal _principal;

        public DistributorsRepository(IDistributorsStore store, ClaimsPrincipal principal)
        {
            _store = store;
            _principal = principal;
        }

        public async Task<List<Distributor>> GetAsync()
        {
            var distributors = await _store.GetAllAsync();
            return _principal switch
            {
                CloudControlApiKeyClaimsPrincipal _ => distributors,
                CloudControlUserClaimsPrincipal u when u.User.DistributorId == DistributorIds.Lucca => distributors,
                CloudControlUserClaimsPrincipal u => distributors.Where(d => d.Id == u.User.DistributorId).ToList(),
                _ => new List<Distributor>()
            };
        }
    }
}
