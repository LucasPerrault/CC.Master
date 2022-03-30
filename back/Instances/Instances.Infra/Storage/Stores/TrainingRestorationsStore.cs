using Environments.Domain.Storage;
using Environments.Infra.Storage.Stores;
using Instances.Domain.Trainings;
using Instances.Domain.Trainings.Filtering;
using Lucca.Core.Api.Abstractions.Paging;
using Lucca.Core.Api.Queryable.Paging;
using Microsoft.EntityFrameworkCore;
using Storage.Infra.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Instances.Infra.Storage.Stores
{
    public class TrainingRestorationsStore : ITrainingRestorationsStore
    {
        private readonly InstancesDbContext _dbContext;
        private readonly IQueryPager _queryPager;

        public TrainingRestorationsStore(InstancesDbContext dbContext, IQueryPager queryPager)
        {
            _dbContext = dbContext;
            _queryPager = queryPager;
        }

        public async Task<TrainingRestoration> CreateAsync(TrainingRestoration restoration)
        {
            _dbContext.Set<TrainingRestoration>().Add(restoration);
            await _dbContext.SaveChangesAsync();
            return restoration;
        }

        public Task<Page<TrainingRestoration>> GetAsync(IPageToken pageToken, TrainingRestorationFilter filter, List<EnvironmentAccessRight> access)
        {
            var restorations = Duplications
                .When(filter.EnvironmentId.HasValue).ApplyWhere(r => r.EnvironmentId == filter.EnvironmentId.Value)
                .WhereHasRights(access);

            return _queryPager.ToPageAsync(restorations, pageToken);
        }

        public async Task<IReadOnlyCollection<TrainingRestoration>> GetByIdsAsync(IReadOnlyCollection<int> ids)
        {
            return await _dbContext.Set<TrainingRestoration>()
                .Where(d => ids.Contains(d.Id))
                .ToListAsync();
        }

        public Task<TrainingRestoration> GetByInstanceDuplicationIdAsync(Guid instanceDuplicationId)
        {
            return Duplications.SingleOrDefaultAsync(d => d.InstanceDuplicationId == instanceDuplicationId);
        }

        public Task<TrainingRestoration> GetActiveByEnvironmentIdAsync(int environmentId)
        {
            return _dbContext.Set<TrainingRestoration>()
                .Where(r => r.EnvironmentId == environmentId)
                .FirstOrDefaultAsync(r => !r.InstanceDuplication.EndedAt.HasValue);
        }

        private IQueryable<TrainingRestoration> Duplications => _dbContext.Set<TrainingRestoration>()
            .Include(d => d.Environment)
            .Include(d => d.InstanceDuplication.Distributor);
    }

    public static class RestorationQueryableExtensions
    {
        public static IQueryable<TrainingRestoration> WhereHasRights
        (
            this IQueryable<TrainingRestoration> restorations,
            List<EnvironmentAccessRight> accessRights
        )
        {
            return EnvironmentQueryableExtensions.FilterOnEnvironmentRights(restorations, r => r.Environment, accessRights);
        }
    }
}
