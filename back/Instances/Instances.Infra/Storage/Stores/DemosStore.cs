using Instances.Domain.Demos;
using Lucca.Core.Api.Abstractions.Paging;
using Lucca.Core.Api.Queryable.Paging;
using Microsoft.EntityFrameworkCore;
using Storage.Infra.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
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

        public Task<Page<Demo>> GetAsync(IPageToken token, Expression<Func<Demo, bool>> filter)
        {
            return _queryPager.ToPageAsync(Demos.Where(filter), token);
        }

        private IQueryable<Demo> Demos => _dbContext.Set<Demo>().Include(d => d.Instance);
    }
}
