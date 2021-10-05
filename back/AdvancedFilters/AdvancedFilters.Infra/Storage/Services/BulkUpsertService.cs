using EFCore.BulkExtensions;
using System.Collections.Generic;
using System.Linq;
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
                IncludeGraph = config.IncludeSubEntities
            };
            await _context.BulkInsertOrUpdateOrDeleteAsync(entities.ToList(), bulkConfig: bc);
        }
    }

    public class BulkUpsertConfig
    {
        public bool IncludeSubEntities { get; set; } = false;
    }
}
