using System.Threading.Tasks;

namespace Instances.Domain.CodeSources
{
    public interface IGithubBranchesStore
    {
        Task CreateForNewSourceCodeAsync(CodeSource codeSource);
    }
}
