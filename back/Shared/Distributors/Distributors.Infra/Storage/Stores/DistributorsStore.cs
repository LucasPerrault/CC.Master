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

        public async Task<Distributor> GetByIdAsync(int id)
        {
            var distributors = await GetAllAsync();
            return distributors.SingleOrDefault(d => d.Id == id);
        }

        public async Task<Distributor> GetByCodeAsync(string code)
        {
            var distributors = await GetAllAsync();
            return distributors.SingleOrDefault(d => d.Code == code);
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
