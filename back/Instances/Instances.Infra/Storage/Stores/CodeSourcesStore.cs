using Instances.Domain.CodeSources;
using Instances.Domain.CodeSources.Filtering;
using Instances.Infra.Storage.Models;
using Microsoft.EntityFrameworkCore;
using Storage.Infra.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Instances.Infra.Storage.Stores
{
    public class CodeSourcesStore : ICodeSourcesStore
    {
        private readonly InstancesDbContext _dbContext;

        public CodeSourcesStore(InstancesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<List<CodeSource>> GetAsync(CodeSourceFilter filter)
        {
            return Get(filter).ToListAsync();
        }

        public Task<CodeSource> GetByIdAsync(int id)
        {
            return Get(CodeSourceFilter.ById(id)).SingleOrDefaultAsync();
        }

        public async Task<CodeSource> CreateAsync(CodeSource codeSource)
        {
            _dbContext.Add(StoredCodeSource.FromCodeSource(codeSource));
            await _dbContext.SaveChangesAsync();
            return codeSource;
        }

        public async Task UpdateLifecycleAsync(CodeSource codeSource, CodeSourceLifecycleStep lifecycleStep)
        {
            var stored = await _dbContext.Set<StoredCodeSource>().SingleAsync(s => s.Id == codeSource.Id);
            stored.Lifecycle = lifecycleStep;
            await _dbContext.SaveChangesAsync();

            codeSource.Lifecycle = lifecycleStep;
        }

        public Task AddProductionVersionAsync(CodeSource codeSource, CodeSourceProductionVersion productionVersion)
        {
            productionVersion.CodeSourceId = codeSource.Id;
            _dbContext.Add(productionVersion);
            return _dbContext.SaveChangesAsync();
        }

        private IQueryable<CodeSource> Get(CodeSourceFilter filter)
        {
            return _dbContext
                .Set<StoredCodeSource>()
                .Include(cs => cs.ProductionVersions)
                .Include(cs => cs.Config)
                .WhereMatches(filter)
                .Select(cs => cs.ToCodeSource());
        }
    }

    internal static class CodeSourceQueryableExtensions
    {
        public static IQueryable<StoredCodeSource> WhereMatches(this IQueryable<StoredCodeSource> codeSources, CodeSourceFilter filter)
        {
            return codeSources
                .WhenNotNullOrEmpty(filter.Search).ApplyWhere(cs => cs.Name.Contains(filter.Search))
                .WhenNotNullOrEmpty(filter.Code).ApplyWhere(cs => cs.Code == filter.Code)
                .WhenNotEmpty(filter.Lifecycle).ApplyWhere(cs => filter.Lifecycle.Contains(cs.Lifecycle))
                .WhenNotEmpty(filter.Id).ApplyWhere(cs => filter.Id.Contains(cs.Id));
        }
    }
}
