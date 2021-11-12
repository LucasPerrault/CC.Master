using Cache.Abstractions;
using Distributors.Domain;
using Distributors.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Distributors.Infra.Storage.Stores
{

    public class DistributorsCache : InMemoryCache<List<Distributor>>
    {
        public DistributorsCache() : base(TimeSpan.FromMinutes(10)) { }
    }

    public class DistributorsStore : IDistributorsStore
    {
        private readonly DistributorsDbContext _dbContext;
        private readonly DistributorsCache _distributorsCache;

        public DistributorsStore(DistributorsDbContext dbContext, DistributorsCache distributorsCache)
        {
            _dbContext = dbContext;
            _distributorsCache = distributorsCache;
        }

        public Task<Distributor> GetActiveByIdAsync(int id)
        {
            return GetDistributorWhereAsync(d => d.Id == id);
        }

        public Task<Distributor> GetActiveByCodeAsync(string code)
        {
            return GetDistributorWhereAsync(d => d.Code == code);
        }

        private async Task<Distributor> GetDistributorWhereAsync(Func<Distributor, bool> predicate)
        {
            var distributors = await GetAllAsync();
            return distributors.Where(d => d.IsActive).SingleOrDefault(predicate);
        }

        public async Task<List<Distributor>> GetAllAsync()
        {
            if (_distributorsCache.TryGet(out var cachedDistributors))
            {
                return cachedDistributors;
            }

            var distributors = await _dbContext.Set<Distributor>().ToListAsync();
            _distributorsCache.Cache(distributors);
            return distributors;
        }
    }
}
