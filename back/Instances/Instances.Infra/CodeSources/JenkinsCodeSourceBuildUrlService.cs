using Instances.Domain.CodeSources;
using System.Collections.Generic;
using System.Text;

namespace Instances.Infra.CodeSources
{
    public class JenkinsCodeSourceBuildUrlService : ICodeSourceBuildUrlService
    {
        private readonly static HashSet<string> ValidStringsBuildNumber = new HashSet<string>(capacity: 2) { "lastSuccessfulBuild", "lastBuild" };

        public bool IsValidBuildNumber(string builderNumber)
        {
            if (string.IsNullOrEmpty(builderNumber))
            {
                return true;
            }
            if (ValidStringsBuildNumber.Contains(builderNumber))
            {
                return true;
            }
            return int.TryParse(builderNumber, out _);
        }

        public string GenerateBuildUrl(CodeSource codeSource, string branchName, string buildNumber)
        {
            var sb = new StringBuilder(codeSource.JenkinsProjectUrl);
            if (codeSource.JenkinsProjectUrl[codeSource.JenkinsProjectUrl.Length - 1] == '/')
            {
                --sb.Length;
            }
            sb.Append("/job/");
            sb.Append(branchName);
            sb.Append("/");
            if (string.IsNullOrEmpty(buildNumber))
            {
                sb.Append("lastSuccessfulBuild");
            }
            else
            {
                sb.Append(buildNumber);
            }
            return sb.ToString();
        }
    }
}
