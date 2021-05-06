using Instances.Domain.Demos;
using Instances.Domain.Demos.Filtering;
using Lucca.Core.Api.Abstractions.Paging;
using Lucca.Core.Api.Queryable.Paging;
using Microsoft.EntityFrameworkCore;
using Rights.Domain.Filtering;
using Storage.Infra.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Tools;

namespace Instances.Infra.Storage.Stores
{
    public class DemosStore : IDemosStore
    {
        private readonly InstancesDbContext _dbContext;
        private readonly IQueryPager _queryPager;

        public DemosStore(InstancesDbContext dbContext, IQueryPager queryPager)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _queryPager = queryPager ?? throw new ArgumentNullException(nameof(queryPager));
        }

        public Task<List<Demo>> GetAsync(DemoFilter filter, AccessRight access)
        {
            return Get(filter, access).ToListAsync();
        }

        public Task<Page<Demo>> GetAsync(IPageToken pageToken, DemoFilter filter, AccessRight access)
        {
            return _queryPager.ToPageAsync(Get(filter, access), pageToken);
        }

        public Task<Demo> GetActiveByIdAsync(int id, AccessRight access)
        {
            var isActiveFilter = new DemoFilter { IsActive = BoolCombination.TrueOnly };

            return Get(isActiveFilter, access)
                .SingleOrDefaultAsync(d => d.Id == id);
        }

        public async Task DeleteAsync(Demo demo)
        {
            demo.IsActive = false;
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateDeletionScheduleAsync(IEnumerable<DemoDeletionSchedule> schedules)
        {
            foreach (var schedule in schedules)
            {
                schedule.Demo.DeletionScheduledOn = schedule.DeletionScheduledOn;
            }
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateCommentAsync(Demo demo, string comment)
        {
            demo.Comment = comment;
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Demo> CreateAsync(Demo demo)
        {
            await _dbContext.Set<Demo>().AddAsync(demo);
            await _dbContext.SaveChangesAsync();
            return demo;
        }

        public Task<Dictionary<string, int>> GetNumberOfActiveDemosByCluster()
        {
            // On ne passe pas par GetActiveDemos pour bénéficier (on espère) du group by en sql
            return Demos.Where(d => d.IsActive).GroupBy(d => d.Instance.Cluster).ToDictionaryAsync(g => g.Key, g => g.Count());
        }

        private IQueryable<Demo> Get(DemoFilter filter, AccessRight access)
        {
            return Demos.Where(ToExpression(filter, access));
        }

        private IQueryable<Demo> Demos => _dbContext.Set<Demo>()
            .Include(d => d.Instance)
            .Include(d => d.Author);

        private Expression<Func<Demo, bool>> ToExpression(DemoFilter filter, AccessRight access)
        {
            var filters = new List<Expression<Func<Demo, bool>>>();

            if (filter.IsActive != BoolCombination.Both)
            {
                var boolean = filter.IsActive.ToBoolean();
                filters.Add(d => d.IsActive == boolean);
            }

            if (filter.IsTemplate != BoolCombination.Both)
            {
                var boolean = filter.IsTemplate.ToBoolean();
                filters.Add(d => d.IsTemplate == boolean);
            }

            if (filter.IsProtected != BoolCombination.Both)
            {
                var boolean = filter.IsProtected.ToBoolean();
                filters.Add(d => d.Instance.IsProtected == boolean);
            }

            if(!string.IsNullOrEmpty(filter.Subdomain))
            {
                filters.Add(d => d.Subdomain == filter.Subdomain);
            }

            if(!string.IsNullOrEmpty(filter.Search))
            {
                filters.Add(d => d.Subdomain.Contains(filter.Search));
            }

            if(!string.IsNullOrEmpty(filter.DistributorId))
            {
                filters.Add(d => d.DistributorID == filter.DistributorId);
            }

            if(filter.AuthorId != null)
            {
                filters.Add(d => d.AuthorId == filter.AuthorId.Value);
            }

            filters.Add(ToRightExpression(access));

            return filters.ToArray().CombineSafely();
        }

        private Expression<Func<Demo, bool>> ToRightExpression(AccessRight access)
        {
            return access switch
            {
                NoAccessRight _ => _ => false,
                DistributorCodeAccessRight r => d => d.Distributor.Code == r.DistributorCode || d.IsTemplate,
                AllAccessRight _ => _ => true,
                _ => throw new ApplicationException($"Unknown type of demo filter right {access}")
            };
        }
    }
}
