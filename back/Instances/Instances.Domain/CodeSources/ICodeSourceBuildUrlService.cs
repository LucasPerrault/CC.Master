namespace Instances.Domain.CodeSources
{
    public interface ICodeSourceBuildUrlService
    {
        bool IsValidBuildNumber(string builderNumber);

        string GenerateBuildUrl(CodeSource codeSource, string branchName, string buildNumber);

    }
}
