using Instances.Domain.Demos;
using Lucca.Core.Api.Abstractions.Paging;
using Lucca.Core.Api.Queryable.Paging;
using Lucca.Core.Shared.Domain.Expressions;
using Microsoft.EntityFrameworkCore;
using Storage.Infra.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

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

        public Task<Page<Demo>> GetAsync(IPageToken token, params Expression<Func<Demo, bool>>[] filters)
        {
            return GetAsync(token, filters.CombineSafely());
        }

        public Task<IQueryable<Demo>> GetActiveAsync(params Expression<Func<Demo, bool>>[] filters)
        {
            return GetActiveAsync(filters.CombineSafely());
        }

        private Task<IQueryable<Demo>> GetActiveAsync(Expression<Func<Demo, bool>> filter)
        {
            return GetAsync(filter.SmartAndAlso(d => d.IsActive));
        }

        public async Task DeleteAsync(Demo demo)
        {
            demo.IsActive = false;
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateDeletionScheduleAsync(Demo demo, DateTime deletionScheduledOn)
        {
            demo.DeletionScheduledOn = deletionScheduledOn;
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Page<Demo>> GetAsync(IPageToken token, Expression<Func<Demo, bool>> filter)
        {
            return await _queryPager.ToPageAsync(await GetAsync(filter), token);
        }

        public Task<IQueryable<Demo>> GetAsync(params Expression<Func<Demo, bool>>[] filters)
        {
            return Task.FromResult(Demos.Where(filters.CombineSafely()));
        }

        public Task<Demo> GetByInstanceIdAsync(int instanceId)
        {
            return Demos.SingleOrDefaultAsync(d => d.InstanceID == instanceId);
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

        private IQueryable<Demo> Demos => _dbContext.Set<Demo>().Include(d => d.Instance);
    }
}
