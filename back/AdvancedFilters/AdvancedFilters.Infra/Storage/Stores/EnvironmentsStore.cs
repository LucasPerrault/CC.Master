using AdvancedFilters.Domain.Instance.Filters;
using AdvancedFilters.Domain.Instance.Interfaces;
using AdvancedFilters.Domain.Instance.Models;
using Lucca.Core.Api.Abstractions.Paging;
using Lucca.Core.Api.Queryable.Paging;
using Microsoft.EntityFrameworkCore;
using Storage.Infra.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdvancedFilters.Infra.Storage.Stores
{
    public class EnvironmentsStore : IEnvironmentsStore
    {
        private readonly AdvancedFiltersDbContext _dbContext;
        private readonly IQueryPager _queryPager;

        public EnvironmentsStore(AdvancedFiltersDbContext dbContext, IQueryPager queryPager)
        {
            _dbContext = dbContext;
            _queryPager = queryPager;
        }

        public Task<Page<Environment>> GetAsync(IPageToken pageToken, EnvironmentFilter filter)
        {
            var envs = Get(filter);
            return _queryPager.ToPageAsync(envs, pageToken);
        }

        public Task<List<Environment>> GetAsync(EnvironmentFilter filter)
        {
            var envs = Get(filter);
            return envs.ToListAsync();
        }

        private IQueryable<Environment> Get(EnvironmentFilter filter)
        {
            return Environments
                .WhereMatches(filter);
        }

        private IQueryable<Environment> Environments => _dbContext.Set<Environment>();
    }

    internal static class EnvironmentQueryableExtensions
    {
        public static IQueryable<Environment> WhereMatches(this IQueryable<Environment> envs, EnvironmentFilter filter)
        {
            return envs
                .WhenNotNullOrEmpty(filter.Search).ApplyWhere(e => e.Subdomain.Contains(filter.Search))
                .Apply(filter.Subdomain).To(e => e.Subdomain)
                .Apply(filter.Domain).To(e => e.Domain)
                .Apply(filter.IsActive).To(e => e.IsActive);
        }
    }
}
