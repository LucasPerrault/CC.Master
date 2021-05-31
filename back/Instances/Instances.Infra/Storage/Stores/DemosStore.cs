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

        public Task DeleteAsync(Demo demo)
        {
            return DeleteAsync(new [] { demo });
        }

        public async Task DeleteAsync(IEnumerable<Demo> demos)
        {
            foreach (var demo in demos)
            {
                demo.IsActive = false;
            }
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
            return Demos
                .WhereMatches(filter)
                .Where(ToRightExpression(access));
        }

        private IQueryable<Demo> Demos => _dbContext.Set<Demo>()
            .Include(d => d.Instance)
            .Include(d => d.Author);


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

    internal static class DemoQueryableExtensions
    {
        public static IQueryable<Demo> WhereMatches(this IQueryable<Demo> demos, DemoFilter filter)
        {
            return demos
                .Apply(filter.IsActive).To(d => d.IsActive)
                .Apply(filter.IsProtected).To(d => d.Instance.IsProtected)
                .Apply(filter.IsTemplate).To(d => d.IsTemplate)
                .WhenNotNullOrEmpty(filter.Search).ApplyWhere(d => d.Subdomain.Contains(filter.Search))
                .WhereStringCompares(filter.Subdomain, d => d.Subdomain)
                .WhenNotNullOrEmpty(filter.DistributorId).ApplyWhere(d => d.DistributorID == filter.DistributorId)
                .WhenHasValue(filter.AuthorId).ApplyWhere(d => d.AuthorId == filter.AuthorId.Value);
        }
    }
}

