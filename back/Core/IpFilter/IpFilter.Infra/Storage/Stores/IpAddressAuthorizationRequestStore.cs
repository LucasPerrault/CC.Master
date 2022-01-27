using IpFilter.Domain;
using Microsoft.EntityFrameworkCore;
using Storage.Infra.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tools;

namespace IpFilter.Infra.Storage.Stores
{
    public class IpFilterAuthorizationRequestStore : IIpFilterAuthorizationRequestStore
    {
        private readonly IpFilterDbContext _dbContext;
        private readonly ITimeProvider _time;

        public IpFilterAuthorizationRequestStore(IpFilterDbContext dbContext, ITimeProvider time)
        {
            _dbContext = dbContext;
            _time = time;
        }

        public Task<List<IpFilterAuthorizationRequest>> GetAsync(IpFilterAuthorizationRequestFilter filter)
        {
            return _dbContext.Set<IpFilterAuthorizationRequest>().WhereMatches(filter).ToListAsync();
        }

        public Task<IpFilterAuthorizationRequest> FirstOrDefaultAsync(IpFilterAuthorizationRequestFilter filter)
        {
            return _dbContext.Set<IpFilterAuthorizationRequest>()
                .WhereMatches(filter)
                .OrderByDescending(r => r.ExpiresAt)
                .FirstOrDefaultAsync();
        }

        public async Task<IpFilterAuthorizationRequest> CreateAsync(IpFilterAuthorizationRequest authorizationRequest)
        {
            await _dbContext.AddAsync(authorizationRequest);
            await _dbContext.SaveChangesAsync();
            return authorizationRequest;
        }

        public Task RevokeAsync(IpFilterAuthorizationRequest validRequest)
        {
            validRequest.RevokedAt = _time.Now();
            return _dbContext.SaveChangesAsync();
        }
    }

    public static class IpFilterAuthorizationRequestQueryableExtensions
    {
        public static IQueryable<IpFilterAuthorizationRequest> WhereMatches(this IQueryable<IpFilterAuthorizationRequest> requests, IpFilterAuthorizationRequestFilter filter)
        {
            return requests
                .WhenNotNullOrEmpty(filter.Addresses).ApplyWhere(c => filter.Addresses.Contains(c.Address))
                .WhenHasValue(filter.UserId).ApplyWhere(c => c.UserId == filter.UserId.Value)
                .WhenHasValue(filter.Code).ApplyWhere(c => c.Code == filter.Code)
                .Apply(filter.RevokedAt).To(c => c.RevokedAt)
                .Apply(filter.CreatedAt).To(c => c.CreatedAt)
                .Apply(filter.ExpiresAt).To(c => c.ExpiresAt);
        }
    }
}
