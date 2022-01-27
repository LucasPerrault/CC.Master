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
    public class DistributorDomainsCache : InMemoryCache<List<DistributorDomain>>
    {
        public DistributorDomainsCache() : base(TimeSpan.FromMinutes(30))
        { }
    }

    public class DistributorDomainsStore : IDistributorDomainsStore
    {
        private readonly DistributorsDbContext _dbContext;
        private readonly DistributorDomainsCache _cache;

        public DistributorDomainsStore(DistributorsDbContext dbContext, DistributorDomainsCache cache)
        {
            _dbContext = dbContext;
            _cache = cache;
        }

        public async Task<List<DistributorDomain>> GetByDistributorId(int distributorId)
        {
            return (await GetAsync())
                .Where(d => d.DistributorId == distributorId)
                .ToList();
        }

        private async Task<List<DistributorDomain>> GetAsync()
        {
            if (_cache.TryGet(out var cachedValues))
            {
                return cachedValues;
            }

            var values = await _dbContext.Set<DistributorDomain>().ToListAsync();
            _cache.Cache(values);
            return values;
        }
    }
}
