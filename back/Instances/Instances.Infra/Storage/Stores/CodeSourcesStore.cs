using Instances.Domain.CodeSources;
using Instances.Domain.CodeSources.Filtering;
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
                Items = page.Items,
                Next = page.Next,
                Prev = page.Prev
            };
        }

        public Task<List<CodeSource>> GetAsync(CodeSourceFilter filter)
        {
            return Get(filter)
                .ToListAsync();
        }

        public async Task<CodeSource> CreateAsync(CodeSource codeSource)
        {
            _dbContext.Add(codeSource);
            await _dbContext.SaveChangesAsync();
            return codeSource;
        }

        public async Task UpdateLifecycleAsync(CodeSource codeSource, CodeSourceLifecycleStep lifecycleStep)
        {
            var stored = await _dbContext.Set<CodeSource>().SingleAsync(s => s.Id == codeSource.Id);
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

        public Task ReplaceProductionArtifactsAsync(CodeSource codeSource, IEnumerable<CodeSourceArtifacts> codeSourceArtifacts)
        {
            _dbContext
                .RemoveRange(
                    _dbContext
                        .Set<CodeSourceArtifacts>()
                        .Where(c => c.CodeSourceId == codeSource.Id)
                );

            _dbContext.AddRange(codeSourceArtifacts.Select(c =>
            {
                c.CodeSourceId = codeSource.Id;
                return c;
            }));
            return _dbContext.SaveChangesAsync();
        }

        private IQueryable<CodeSource> Get(CodeSourceFilter filter)
        {
            return _dbContext
                .Set<CodeSource>()
                .Include(cs => cs.Repo)
                .Include(cs => cs.ProductionVersions)
                .Include(cs => cs.Config)
                .WhereMatches(filter);
        }

        public Task<List<CodeSourceArtifacts>> GetArtifactsAsync(int codeSourceId)
        {
            return _dbContext
                .Set<CodeSourceArtifacts>()
                .Where(c => c.CodeSourceId == codeSourceId)
                .ToListAsync();
        }

        public Task<List<CodeSourceArtifacts>> GetArtifactsAsync(IEnumerable<int> codeSourceIds, CodeSourceArtifactType codeSourceArtifactType)
        {
            return _dbContext
                .Set<CodeSourceArtifacts>()
                .Where(c => c.ArtifactType == codeSourceArtifactType && codeSourceIds.Contains(c.CodeSourceId))
                .ToListAsync();
        }
    }

    internal static class CodeSourceQueryableExtensions
    {
        public static IQueryable<CodeSource> WhereMatches(this IQueryable<CodeSource> codeSources, CodeSourceFilter filter)
        {
            return codeSources
                .WhenNotNullOrEmpty(filter.Search).ApplyWhere(cs => cs.Name.Contains(filter.Search))
                .WhenNotNullOrEmpty(filter.Code).ApplyWhere(cs => cs.Code == filter.Code)
                .WhenNotNullOrEmpty(filter.RepoIds).ApplyWhere(cs => filter.RepoIds.Contains(cs.RepoId))
                .WhenNotNullOrEmpty(filter.Lifecycle).ApplyWhere(cs => filter.Lifecycle.Contains(cs.Lifecycle))
                .WhenNotNullOrEmpty(filter.ExcludedLifecycle).ApplyWhere(cs => !filter.ExcludedLifecycle.Contains(cs.Lifecycle))
                .WhenNotNullOrEmpty(filter.Type).ApplyWhere(cs => filter.Type.Contains(cs.Type))
                .WhenNotNullOrEmpty(filter.Id).ApplyWhere(cs => filter.Id.Contains(cs.Id));
        }
    }
}
