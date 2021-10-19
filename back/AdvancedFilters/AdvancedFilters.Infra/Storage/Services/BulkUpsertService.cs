using EFCore.BulkExtensions;
using Storage.Infra.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AdvancedFilters.Infra.Storage.Services
{
    public interface IBulkUpsertService
    {
        Task InsertOrUpdateOrDeleteAsync<T>(IReadOnlyCollection<T> entities, BulkUpsertConfig config) where T : class;
    }
    public class BulkUpsertService : IBulkUpsertService
    {
        private readonly AdvancedFiltersDbContext _context;

        public BulkUpsertService(AdvancedFiltersDbContext context)
        {
            _context = context;
        }

        public async Task InsertOrUpdateOrDeleteAsync<T>(IReadOnlyCollection<T> entities, BulkUpsertConfig config)
            where T : class
        {
            var bc = new BulkConfig
            {
                IncludeGraph = config.IncludeSubEntities,
            };
            bc.SetSynchronizeFilter(GetSyncFilter<T>(config.Filter));
            await _context.BulkInsertOrUpdateOrDeleteAsync(entities.ToList(), bulkConfig: bc);
        }

        private Expression<Func<T,bool>> GetSyncFilter<T>(UpsertFilter configFilter) where T : class
        {
            return configFilter switch
            {
                UpsertEverythingFilter _ => null,
                UpsertForEnvironmentsFilter<T> environmentsFilter => environmentsFilter.GetEnvId.Chain(id => environmentsFilter.EnvIds.Contains(id)),
                _ => throw new ApplicationException($"UpsertFilter not supported : {typeof(UpsertFilter)}")
            };
        }
    }

    public class BulkUpsertConfig
    {
        public bool IncludeSubEntities { get; set; } = false;
        public UpsertFilter Filter { get; set; } = UpsertFilter.Everything();
    }

    public abstract class UpsertFilter
    {
        public static UpsertFilter Everything() => new UpsertEverythingFilter();
        public static UpsertFilter ForEnvironments<T>(HashSet<int> envIds, Expression<Func<T, int>> getId) => new UpsertForEnvironmentsFilter<T>(envIds, getId);
    }

    public class UpsertEverythingFilter : UpsertFilter
    { }

    public class UpsertForEnvironmentsFilter<T> : UpsertFilter
    {
        public HashSet<int> EnvIds { get; }
        public Expression<Func<T, int>> GetEnvId { get; }

        public UpsertForEnvironmentsFilter(HashSet<int> envIds, Expression<Func<T, int>> getEnvId)
        {
            EnvIds = envIds;
            GetEnvId = getEnvId;
        }
    }
}
