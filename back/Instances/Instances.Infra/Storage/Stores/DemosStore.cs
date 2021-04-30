using Instances.Domain.Demos;
using Instances.Domain.Demos.Filtering;
using Lucca.Core.Api.Abstractions.Paging;
using Lucca.Core.Api.Queryable.Paging;
using Microsoft.EntityFrameworkCore;
using Storage.Infra.Stores;
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

        public Task<IQueryable<Demo>> GetAsync(DemoFilter filter)
        {
            return Task.FromResult(Demos.Where(ToExpression(filter)));
        }

        public async Task<Page<Demo>> GetAsync(IPageToken pageToken, DemoFilter filter)
        {
            return await _queryPager.ToPageAsync(await GetAsync(filter), pageToken);
        }

        public Task<Demo> GetActiveByIdAsync(int id, DemoAccess demoAccess)
        {
            return Demos
                .Where(d => d.IsActive)
                .Where(ToRightExpression(demoAccess))
                .SingleOrDefaultAsync(d => d.Id == id);
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

        private Expression<Func<Demo, bool>> ToExpression(DemoFilter filter)
        {
            var filters = new List<Expression<Func<Demo, bool>>>();

            if (filter.IsActive != BoolCombination.Both)
            {
                var boolean = ToBoolean(filter.IsActive);
                filters.Add(d => d.IsActive == boolean);
            }

            if(!string.IsNullOrEmpty(filter.Subdomain))
            {
                filters.Add(d => d.Subdomain == filter.Subdomain);
            }

            filters.Add(ToRightExpression(filter.Access));

            return filters.ToArray().CombineSafely();
        }

        private bool ToBoolean(BoolCombination boolCombination)
        {
            return boolCombination switch
            {
                BoolCombination.TrueOnly => true,
                BoolCombination.FalseOnly => false,
                _ => throw new ApplicationException($"Unexpected bool combination value {boolCombination}")
            };
        }

        private Expression<Func<Demo, bool>> ToRightExpression(DemoAccess access)
        {
            return access switch
            {
                NoDemosAccess _ => _ => false,
                DistributorDemosAccess r => d => d.Distributor.Code == r.DistributorCode || d.IsTemplate,
                AllDemosAccess _ => _ => true,
                _ => throw new ApplicationException($"Unknown type of demo filter right {access}")
            };
        }
    }
}
