using Instances.Domain.CodeSources.Filtering;
using Instances.Domain.Github.Models;
using System.Threading.Tasks;

namespace Instances.Domain.Github
{
    public interface IGithubPullRequestsStore
    {
        Task<GithubPullRequest> CreateAsync(GithubPullRequest pullRequest);
        Task<GithubPullRequest> GetFirstAsync(GithubPullRequestFilter githubPullRequestFilter);
        Task<GithubPullRequest> UpdateAsync(GithubPullRequest pullRequest);
    }
}
