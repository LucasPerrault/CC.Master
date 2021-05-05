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

        public Task<List<Demo>> GetAsync(DemoFilter filter, DemoAccess access)
        {
            return Get(filter, access).ToListAsync();
        }

        public Task<Page<Demo>> GetAsync(IPageToken pageToken, DemoFilter filter, DemoAccess access)
        {
            return _queryPager.ToPageAsync(Get(filter, access), pageToken);
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

        private IQueryable<Demo> Get(DemoFilter filter, DemoAccess access)
        {
            return Demos.Where(ToExpression(filter, access));
        }

        private IQueryable<Demo> Demos => _dbContext.Set<Demo>()
            .Include(d => d.Instance)
            .Include(d => d.Author);

        private Expression<Func<Demo, bool>> ToExpression(DemoFilter filter, DemoAccess access)
        {
            var filters = new List<Expression<Func<Demo, bool>>>();

            if (filter.IsActive != BoolCombination.Both)
            {
                var boolean = ToBoolean(filter.IsActive);
                filters.Add(d => d.IsActive == boolean);
            }

            if (filter.IsTemplate != BoolCombination.Both)
            {
                var boolean = ToBoolean(filter.IsTemplate);
                filters.Add(d => d.IsTemplate == boolean);
            }

            if (filter.IsProtected != BoolCombination.Both)
            {
                var boolean = ToBoolean(filter.IsProtected);
                filters.Add(d => d.Instance.IsProtected == boolean);
            }

            if(!string.IsNullOrEmpty(filter.Subdomain))
            {
                filters.Add(d => d.Subdomain == filter.Subdomain);
            }

            if(!string.IsNullOrEmpty(filter.Search))
            {
                filters.Add(d => filter.Search.Contains(d.Subdomain));
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
