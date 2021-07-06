using Instances.Domain.CodeSources.Filtering;
using Lucca.Core.Api.Abstractions.Paging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Instances.Domain.CodeSources
{
    public interface ICodeSourcesStore
    {
        Task<Page<CodeSource>> GetAsync(IPageToken pageToken, CodeSourceFilter filter);
        Task<List<CodeSource>> GetAsync(CodeSourceFilter filter);
        Task<CodeSource> CreateAsync(CodeSource codeSource);
        Task UpdateLifecycleAsync(CodeSource codeSource, CodeSourceLifecycleStep lifecycleStep);
        Task AddProductionVersionAsync(CodeSource codeSource, CodeSourceProductionVersion productionVersion);
    }
}
