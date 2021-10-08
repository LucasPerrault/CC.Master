using Instances.Domain.CodeSources;
using Instances.Domain.CodeSources.Filtering;
using Instances.Domain.Github;
using Instances.Domain.Github.Models;
using System.Threading.Tasks;

namespace Instances.Application.Instances
{
    public class GithubPullRequestsRepository : IGithubPullRequestsRepository
    {
        private readonly IGithubPullRequestsStore _githubPullRequestsStore;

        public GithubPullRequestsRepository(IGithubPullRequestsStore githubPullRequestsStore)
        {
            _githubPullRequestsStore = githubPullRequestsStore;
        }

        public Task<GithubPullRequest> CreateAsync(GithubPullRequest pullRequest)
            => _githubPullRequestsStore.CreateAsync(pullRequest);

        public Task<GithubPullRequest> GetByNumberAsync(CodeSource codeSource, int pullRequestNumber)
            => _githubPullRequestsStore.GetFirstAsync(new GithubPullRequestFilter
            {
                CodeSourceId = codeSource.Id,
                Number = pullRequestNumber
            });

        public Task<GithubPullRequest> UpdateAsync(GithubPullRequest pullRequest)
            => _githubPullRequestsStore.UpdateAsync(pullRequest);
    }
}
