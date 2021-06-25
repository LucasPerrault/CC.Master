using System.Collections.Generic;
using System.Threading.Tasks;

namespace Instances.Domain.CodeSources
{
    public interface ICodeSourceFetcherService
    {
        Task<IEnumerable<CodeSource>> FetchAsync(string repoUrl);
    }
}
