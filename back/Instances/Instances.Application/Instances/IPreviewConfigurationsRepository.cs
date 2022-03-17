using Instances.Domain.CodeSources;
using Instances.Domain.Github.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Instances.Application.Instances
{
    public interface IPreviewConfigurationsRepository
    {
        Task CreateByBranchAsync(IEnumerable<GithubBranch> branches, IEnumerable<CodeSource> codeSources);
        Task CreateByBranchAsync(IEnumerable<GithubBranch> branches, CodeSource codeSource);
        Task DeleteByBranchAsync(GithubBranch branch);
        Task CreateByPullRequestAsync(GithubPullRequest pullRequest, GithubBranch originBranch, IEnumerable<CodeSource> codeSources);
    }
}
