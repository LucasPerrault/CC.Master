using Instances.Domain.CodeSources.Filtering;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Instances.Domain.CodeSources
{
    public interface ICodeSourcesStore
    {
        Task<List<CodeSource>> GetAsync(CodeSourceFilter filter);
        Task<CodeSource> GetByIdAsync(int id);
        Task<CodeSource> CreateAsync(CodeSource codeSource);
        Task UpdateLifecycleAsync(CodeSource codeSource, CodeSourceLifecycleStep lifecycleStep);
        Task<CodeSource> GetNonDeletedByCodeAsync(string code);
        Task AddProductionVersionAsync(CodeSource codeSource, CodeSourceProductionVersion productionVersion);
    }
}
