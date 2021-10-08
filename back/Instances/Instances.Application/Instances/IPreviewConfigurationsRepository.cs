using Instances.Domain.Github.Models;
using System.Threading.Tasks;

namespace Instances.Application.Instances
{
    public interface IPreviewConfigurationsRepository
    {
        Task CreateByBranchAsync(GithubBranch branch);
        Task DeleteByBranchAsync(GithubBranch branch);
        Task CreateByPullRequestAsync(GithubPullRequest pullRequest, GithubBranch originBranch);
    }
}
