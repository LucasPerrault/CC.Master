using Instances.Domain.CodeSources;
using Instances.Domain.CodeSources.Filtering;
using Instances.Infra.Storage.Models;
using Lucca.Core.Api.Abstractions.Paging;
using Lucca.Core.Api.Queryable.Paging;
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
        private readonly IQueryPager _queryPager;

        public CodeSourcesStore(InstancesDbContext dbContext, IQueryPager queryPager)
        {
            _dbContext = dbContext;
            _queryPager = queryPager;
        }

        public async Task<Page<CodeSource>> GetAsync(IPageToken pageToken, CodeSourceFilter filter)
        {
            var page = await _queryPager.ToPageAsync(Get(filter), pageToken);
            return new Page<CodeSource>
            {
                Count = page.Count,
                Items = page.Items.Select(s => s.ToCodeSource()),
                Next = page.Next,
                Prev = page.Prev
            };
        }

        public Task<List<CodeSource>> GetAsync(CodeSourceFilter filter)
        {
            return Get(filter)
                .Select(s => s.ToCodeSource())
                .ToListAsync();
        }

        public async Task<CodeSource> CreateAsync(CodeSource codeSource)
        {
            var storedCodeSource = StoredCodeSource.FromCodeSource(codeSource);
            _dbContext.Add(storedCodeSource);
            await _dbContext.SaveChangesAsync();
            codeSource.Id = storedCodeSource.Id;
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

        private IQueryable<StoredCodeSource> Get(CodeSourceFilter filter)
        {
            return _dbContext
                .Set<StoredCodeSource>()
                .Include(cs => cs.ProductionVersions)
                .Include(cs => cs.Config)
                .WhereMatches(filter);
        }
    }

    internal static class CodeSourceQueryableExtensions
    {
        public static IQueryable<StoredCodeSource> WhereMatches(this IQueryable<StoredCodeSource> codeSources, CodeSourceFilter filter)
        {
            return codeSources
                .WhenNotNullOrEmpty(filter.Search).ApplyWhere(cs => cs.Name.Contains(filter.Search))
                .WhenNotNullOrEmpty(filter.Code).ApplyWhere(cs => cs.Code == filter.Code)
                .WhenNotNullOrEmpty(filter.Lifecycle).ApplyWhere(cs => filter.Lifecycle.Contains(cs.Lifecycle))
                .WhenNotNullOrEmpty(filter.Type).ApplyWhere(cs => filter.Type.Contains(cs.Type))
                .WhenNotNullOrEmpty(filter.Id).ApplyWhere(cs => filter.Id.Contains(cs.Id));
        }
    }
}
