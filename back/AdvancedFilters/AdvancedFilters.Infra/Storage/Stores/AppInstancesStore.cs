using AdvancedFilters.Domain.Instance.Filters;
using AdvancedFilters.Domain.Instance.Interfaces;
using AdvancedFilters.Domain.Instance.Models;
using Lucca.Core.Api.Abstractions.Paging;
using Lucca.Core.Api.Queryable.Paging;
using Microsoft.EntityFrameworkCore;
using Storage.Infra.Extensions;
using System.Linq;
using System.Threading.Tasks;

namespace AdvancedFilters.Infra.Storage.Stores
{
    public class AppInstancesStore : IAppInstancesStore
    {
        private readonly AdvancedFiltersDbContext _dbContext;
        private readonly IQueryPager _queryPager;

        public AppInstancesStore(AdvancedFiltersDbContext dbContext, IQueryPager queryPager)
        {
            _dbContext = dbContext;
            _queryPager = queryPager;
        }

        public Task<Page<AppInstance>> GetAsync(IPageToken pageToken, AppInstanceFilter filter)
        {
            var ais = Get(filter);
            return _queryPager.ToPageAsync(ais, pageToken);
        }

        private IQueryable<AppInstance> Get(AppInstanceFilter filter)
        {
            return AppInstances
                .WhereMatches(filter)
                .AsNoTracking();
        }

        private IQueryable<AppInstance> AppInstances => _dbContext
            .Set<AppInstance>()
            .Include(i => i.Environment).ThenInclude(e => e.LegalUnits).ThenInclude(lu => lu.Establishments);
    }

    internal static class AppInstanceQueryableExtensions
    {
        public static IQueryable<AppInstance> WhereMatches(this IQueryable<AppInstance> ais, AppInstanceFilter filter)
        {
            return ais
                .WhenNotNullOrEmpty(filter.Ids).ApplyWhere(ai => filter.Ids.Contains(ai.Id))
                .WhenNotNullOrEmpty(filter.ApplicationIds).ApplyWhere(ai => filter.ApplicationIds.Contains(ai.ApplicationId))
                .WhenNotNullOrEmpty(filter.EnvironmentIds).ApplyWhere(ai => filter.EnvironmentIds.Contains(ai.EnvironmentId));
        }
    }
}
