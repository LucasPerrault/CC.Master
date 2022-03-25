using Instances.Domain.Github.Models;
using System.Threading.Tasks;

namespace Instances.Application.Instances
{
    public interface IGithubPullRequestsRepository
    {
        Task<GithubPullRequest> CreateAsync(GithubPullRequest pullRequest);
        Task<GithubPullRequest> GetByNumberAsync(int repoId, int pullRequestNumber);
        Task<GithubPullRequest> UpdateAsync(GithubPullRequest pullRequest);
    }
}
