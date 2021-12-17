using IpFilter.Domain;
using Microsoft.EntityFrameworkCore;
using Storage.Infra.Extensions;
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

        public async Task<IReadOnlyCollection<IpFilterAuthorization>> GetAsync(IpFilterAuthorizationFilter filter)
        {
            return await _dbContext.Set<IpFilterAuthorization>().WhereMatches(filter).ToListAsync();
        }

        public async Task<IpFilterAuthorization> CreateAsync(IpFilterAuthorization authorization)
        {
            await _dbContext.Set<IpFilterAuthorization>().AddAsync(authorization);
            await _dbContext.SaveChangesAsync();
            return authorization;
        }

        public Task<bool> ExistsAsync(int requestId)
        {
            return _dbContext.Set<IpFilterAuthorization>().AnyAsync(a => a.RequestId == requestId);
        }
    }

    public static class QueryableExtensions
    {
        public static IQueryable<IpFilterAuthorization> WhereMatches(this IQueryable<IpFilterAuthorization> authorizations, IpFilterAuthorizationFilter filter)
        {
            return authorizations
                .Apply(filter.CreatedAt).To(a => a.CreatedAt)
                .Apply(filter.ExpiresAt).To(f => f.ExpiresAt)
                .WhenHasValue(filter.UserId).ApplyWhere(a => a.UserId == filter.UserId.Value)
                .WhenNotNullOrEmpty(filter.IpAddress).ApplyWhere(a => a.IpAddress == filter.IpAddress);
        }
    }
}
