using Instances.Domain.CodeSources;
using Instances.Domain.Github.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Instances.Application.Instances
{
    public interface IPreviewConfigurationsRepository
    {
        Task CreateByBranchAsync(IEnumerable<GithubBranch> branch, IEnumerable<CodeSource> codeSources);
        Task CreateByBranchAsync(IEnumerable<GithubBranch> branch, CodeSource codeSource);
        Task DeleteByBranchAsync(GithubBranch branch);
        Task CreateByPullRequestAsync(GithubPullRequest pullRequest, GithubBranch originBranch, IEnumerable<CodeSource> codeSources);
    }
}
