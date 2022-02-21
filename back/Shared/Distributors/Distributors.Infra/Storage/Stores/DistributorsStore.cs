using Cache.Abstractions;
using Distributors.Domain;
using Distributors.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Storage.Infra.Extensions;
using Storage.Infra.Querying;

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

            var distributors = await Distributors.ToListAsync();
            _distributorsCache.Cache(distributors);
            return distributors;
        }

        public async Task<List<Distributor>> GetAsync(DistributorFilter filter)
        {
            var distributors = await GetAllAsync();
            return distributors.WhereMatches(filter).ToList();
        }

        private IQueryable<Distributor> Distributors => _dbContext.Set<Distributor>();
    }

    internal static class DistributorsFilterExtensions
    {
        public static IEnumerable<Distributor> WhereMatches(this IEnumerable<Distributor> distributors, DistributorFilter filter)
        {
            return distributors.Search(filter.Search);
        }

        private static IEnumerable<Distributor> Search(this IEnumerable<Distributor> distributors, HashSet<string> words)
        {
            var usableWords = words.Sanitize();
            if (!usableWords.Any())
            {
                return distributors;
            }

            return distributors.Where(distributor => distributor.IsMatchingSearch(words));
        }

        private static bool IsMatchingSearch(this Distributor distributor, HashSet<string> words)
        {
            var distributorWords = distributor.Name.Split(' ');
            return words
                .Where(w => !string.IsNullOrEmpty(w))
                .All(w => distributorWords.Any(t => t.ToLower().StartsWith(w.ToLower())));
        }
    }
}
