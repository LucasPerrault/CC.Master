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
using System.Linq.Expressions;
using System.Threading.Tasks;
using Tools;
using CCEnvironment = Environments.Domain.Environment;

namespace Instances.Infra.Storage.Stores
{
    public class TrainingsStore : ITrainingsStore
    {
        private readonly InstancesDbContext _dbContext;
        private readonly IQueryPager _queryPager;

        public TrainingsStore(InstancesDbContext dbContext, IQueryPager queryPager)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _queryPager = queryPager ?? throw new ArgumentNullException(nameof(queryPager));
        }

        public Task<List<Training>> GetAsync(TrainingFilter filter, List<EnvironmentAccessRight> accessRights)
        {
            return Get(filter, accessRights).ToListAsync();
        }

        public Task<Page<Training>> GetAsync(IPageToken pageToken, TrainingFilter filter, List<EnvironmentAccessRight> accessRights)
        {
            return _queryPager.ToPageAsync(Get(filter, accessRights), pageToken);
        }

        public Task<Training> GetActiveByIdAsync(int id, List<EnvironmentAccessRight> accessRights)
        {
            var isActiveFilter = new TrainingFilter { IsActive = CompareBoolean.TrueOnly };

            return Get(isActiveFilter, accessRights)
                .SingleOrDefaultAsync(d => d.Id == id);
        }

        public async Task DeleteForInstanceAsync(int instanceId)
        {
            var training = await Get(new TrainingFilter
            {
                InstanceId = instanceId,
                IsActive = CompareBoolean.TrueOnly,
            }, EnvironmentAccessRight.Everything).SingleOrDefaultAsync();
            if(training != null)
            {
                await DeleteAsync(training);
            }
        }

        public Task DeleteAsync(Training training)
        {
            return DeleteAsync(new [] { training });
        }

        public async Task DeleteAsync(IEnumerable<Training> trainings)
        {
            foreach (var training in trainings)
            {
                training.IsActive = false;
            }
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Training> CreateAsync(Training training)
        {
            _dbContext.Set<Training>().Add(training);
            await _dbContext.SaveChangesAsync();
            return training;
        }


        private IQueryable<Training> Get(TrainingFilter filter, List<EnvironmentAccessRight> accessRights)
        {
            return Trainings
                .ForRights(accessRights)
                .WhereMatches(filter);
        }

        private IQueryable<Training> Trainings => _dbContext.Set<Training>()
            .Include(d => d.Instance)
            .Include(d => d.TrainingRestoration).ThenInclude(tr => tr.InstanceDuplication)
            .Include(d => d.Environment).ThenInclude(e => e.ActiveAccesses).ThenInclude(a => a.Consumer)
            .Include(d => d.Environment).ThenInclude(e => e.ActiveAccesses).ThenInclude(a => a.Access);

    }

    internal static class TrainingQueryableExtensions
    {
        public static IQueryable<Training> WhereMatches(this IQueryable<Training> trainings, TrainingFilter filter)
        {
            return trainings
                .Apply(filter.IsActive).To(t => t.IsActive)
                .Apply(filter.IsProtected).To(t => t.Instance.IsProtected)
                .Apply(filter.Subdomain).To(t => t.Environment.Subdomain)
                .WhenHasValue(filter.InstanceId).ApplyWhere(t => t.InstanceId == filter.InstanceId.Value)
                .WhenHasValue(filter.EnvironmentId).ApplyWhere(t => t.EnvironmentId == filter.EnvironmentId.Value)
                .WhenHasValue(filter.AuthorId).ApplyWhere(t => t.AuthorId == filter.AuthorId.Value);
        }


        public static IQueryable<Training> ForRights
        (
            this IQueryable<Training> trainings,
            List<EnvironmentAccessRight> accessRights
        )
        {
            var environmentRightsExpression = EnvironmentQueryableExtensions.ForRightsExpression(accessRights);
            Expression<Func<Training, CCEnvironment>> trainingToEnvironment = t => t.Environment;

            var trainingRightsExpression = trainingToEnvironment.Chain(environmentRightsExpression);

            return trainings.Where(trainingRightsExpression);
        }
    }
}
