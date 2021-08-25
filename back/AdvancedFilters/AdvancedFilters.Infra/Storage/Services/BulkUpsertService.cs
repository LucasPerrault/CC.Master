using EFCore.BulkExtensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdvancedFilters.Infra.Storage.Services
{
    public class BulkUpsertService
    {
        private readonly AdvancedFiltersDbContext _context;

        public BulkUpsertService(AdvancedFiltersDbContext context)
        {
            _context = context;
        }

        public async Task InsertOrUpdateOrDeleteAsync<T>(IReadOnlyCollection<T> entities)
            where T : class
        {
            await _context.BulkInsertOrUpdateOrDeleteAsync(entities.ToList());
        }
    }
}
