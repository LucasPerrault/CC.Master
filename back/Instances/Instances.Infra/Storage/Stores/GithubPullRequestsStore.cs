using Instances.Domain.CodeSources.Filtering;
using Instances.Domain.Github;
using Instances.Domain.Github.Models;
using Microsoft.EntityFrameworkCore;
using Storage.Infra.Extensions;
using System.Linq;
using System.Threading.Tasks;

namespace Instances.Infra.Storage.Stores
{
    public class GithubPullRequestsStore : IGithubPullRequestsStore
    {
        private readonly InstancesDbContext _dbContext;

        public GithubPullRequestsStore(InstancesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<GithubPullRequest> CreateAsync(GithubPullRequest pullRequest)
        {
            _dbContext.Add(pullRequest);
            await _dbContext.SaveChangesAsync();
            return pullRequest;
        }

        public async Task<GithubPullRequest> GetFirstAsync(GithubPullRequestFilter filter)
            => await Get(filter).FirstOrDefaultAsync();

        private IQueryable<GithubPullRequest> Get(GithubPullRequestFilter filter)
        {
            return _dbContext
                .Set<GithubPullRequest>()
                .WhereMatches(filter);
        }

        public async Task<GithubPullRequest> UpdateAsync(GithubPullRequest pullRequest)
        {
            _dbContext.Update(pullRequest);
            await _dbContext.SaveChangesAsync();
            return pullRequest;
        }
    }

    internal static class GithubPullRequestQueryableExtensions
    {
        public static IQueryable<GithubPullRequest> WhereMatches(this IQueryable<GithubPullRequest> githubPullRequests, GithubPullRequestFilter filter)
        {
            return githubPullRequests
                .WhenHasValue(filter.CodeSourceId).ApplyWhere(pr => pr.CodeSources.Any(c => c.Id == filter.CodeSourceId))
                .WhenHasValue(filter.Number).ApplyWhere(pr => pr.Number == filter.Number);
        }
    }
}
