using AdvancedFilters.Domain.Filters.Models;
using AdvancedFilters.Domain.Instance.Filters;
using AdvancedFilters.Domain.Instance.Interfaces;
using AdvancedFilters.Domain.Instance.Models;
using AdvancedFilters.Infra.Filters;
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

        public Task<Page<Environment>> SearchAsync(IPageToken pageToken, IAdvancedFilter filter)
        {
            var envs = Environments
                .Filter(filter)
                .AsNoTracking();
            return _queryPager.ToPageAsync(envs, pageToken);
        }

        public Task<List<string>> GetClustersAsync()
        {
            return Environments.Select(e => e.Cluster).Distinct().OrderBy(e => e).ToListAsync();
        }

        private IQueryable<Environment> Get(EnvironmentFilter filter)
        {
            return Environments
                .WhereMatches(filter)
                .AsNoTracking();
        }

        private IQueryable<Environment> Environments => _dbContext
            .Set<Environment>()
            .Include(e => e.LegalUnits).ThenInclude(lu => lu.Country)
            .Include(e => e.LegalUnits).ThenInclude(lu => lu.Establishments)
            .Include(e => e.AppInstances);
    }

    internal static class EnvironmentQueryableExtensions
    {
        public static IQueryable<Environment> WhereMatches(this IQueryable<Environment> envs, EnvironmentFilter filter)
        {
            return envs
                .WhenNotNullOrEmpty(filter.Search).ApplyWhere(e => e.Subdomain.Contains(filter.Search))
                .WhenNotNullOrEmpty(filter.Subdomains).ApplyWhere(e => filter.Subdomains.Contains(e.Subdomain))
                .WhenNotNullOrEmpty(filter.Domains).ApplyWhere(e => filter.Domains.Contains(e.Domain))
                .Apply(filter.IsActive).To(e => e.IsActive);
        }
    }
}
