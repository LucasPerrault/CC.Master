using Instances.Domain.Demos;
using Lucca.Core.Api.Abstractions.Paging;
using Lucca.Core.Api.Queryable.Paging;
using Lucca.Core.Shared.Domain.Expressions;
using Microsoft.EntityFrameworkCore;
using Storage.Infra.Stores;
using System;
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

        public async Task<Page<Demo>> GetAsync(IPageToken token, Expression<Func<Demo, bool>> filter)
        {
            return await _queryPager.ToPageAsync(await GetAsync(filter), token);
        }

        public Task<IQueryable<Demo>> GetAsync(Expression<Func<Demo, bool>> filter)
        {
            return Task.FromResult(Demos.Where(filter));
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

        private IQueryable<Demo> Demos => _dbContext.Set<Demo>().Include(d => d.Instance);
    }
}
