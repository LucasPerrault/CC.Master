using System.Threading.Tasks;

namespace Instances.Domain.CodeSources
{
    public interface ICodeSourceBuildUrlService
    {
        bool IsValidBuildNumber(string builderNumber);

        Task<string> GenerateBuildUrlAsync(CodeSource codeSource, string branchName, string buildNumber);

    }
}
