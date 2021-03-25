using Instances.Domain.Demos;
using Lucca.Core.Api.Abstractions.Paging;
using Lucca.Core.Api.Queryable.Paging;
using Storage.Infra.Stores;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Instances.Infra.Storage.Stores
{
    public class DemoDuplicationsStore : IDemoDuplicationsStore
    {
        private readonly InstancesDbContext _dbContext;
        private readonly IQueryPager _queryPager;

        public DemoDuplicationsStore(InstancesDbContext dbContext, IQueryPager queryPager)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _queryPager = queryPager ?? throw new ArgumentNullException(nameof(queryPager));
        }

        public Task<Page<DemoDuplication>> GetAsync(IPageToken token, params Expression<Func<DemoDuplication, bool>>[] filters)
        {
            return GetAsync(token, filters.CombineSafely());
        }

        public IQueryable<DemoDuplication> GetAllAsync()
        {
            return Duplications;
        }

        public async Task<Page<DemoDuplication>> GetAsync(IPageToken token, Expression<Func<DemoDuplication, bool>> filter)
        {
            return await _queryPager.ToPageAsync(await GetAsync(filter), token);
        }

        public Task<IQueryable<DemoDuplication>> GetAsync(Expression<Func<DemoDuplication, bool>> filter)
        {
            return Task.FromResult(Duplications.Where(filter));
        }

        public async Task<DemoDuplication> CreateAsync(DemoDuplication duplication)
        {
            await _dbContext.Set<DemoDuplication>().AddAsync(duplication);
            await _dbContext.SaveChangesAsync();
            return duplication;
        }

        private IQueryable<DemoDuplication> Duplications => _dbContext.Set<DemoDuplication>();
    }
}
