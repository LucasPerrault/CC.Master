using System.Collections.Generic;
using System.Threading.Tasks;

namespace Instances.Domain.CodeSources
{
    public interface IArtifactsService
    {
        Task<IEnumerable<CodeSourceArtifacts>> GetArtifactsAsync(CodeSource source, string branchName, int buildNumber);
    }
}
