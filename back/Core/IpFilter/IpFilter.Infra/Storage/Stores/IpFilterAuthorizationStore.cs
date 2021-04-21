using IpFilter.Domain;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IpFilter.Infra.Storage.Stores
{
    public class IpFilterAuthorizationStore : IIpFilterAuthorizationStore
    {
        private readonly IpFilterDbContext _dbContext;

        public IpFilterAuthorizationStore(IpFilterDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyCollection<IpFilterAuthorization>> GetByUserAsync(IpFilterUser user)
        {
            return await _dbContext.Set<IpFilterAuthorization>()
                .Where
                    (
                        d =>
                            d.IpAddress == user.IpAddress
                            && d.UserId == user.UserId
                    ).ToListAsync();
        }
    }
}
