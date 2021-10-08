using Instances.Domain.CodeSources;
using Instances.Domain.CodeSources.Filtering;
using Instances.Domain.Github;
using Instances.Domain.Github.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Remote.Infra.Exceptions;
using Remote.Infra.Services;
using Storage.Infra.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Instances.Infra.Storage.Stores
{
    public class GithubBranchesStore : IGithubBranchesStore
    {
        private readonly ILogger<GithubBranchesStore> _logger;
        private readonly RestApiV3HttpClientHelper _httpClientHelper;

        private readonly InstancesDbContext _dbContext;

        public GithubBranchesStore(
            HttpClient httpClient, InstancesDbContext dbContext,
            ILogger<GithubBranchesStore> logger)
        {
            _httpClientHelper = new RestApiV3HttpClientHelper(httpClient, "Legacy cloudcontrol github branches api");
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<GithubBranch> CreateAsync(GithubBranch branch)
        {
            _dbContext.Set<GithubBranch>().Add(branch);
            await _dbContext.SaveChangesAsync();
            return branch;
        }

        public async Task<GithubBranch> GetFirstAsync(GithubBranchFilter githubBranchFilter)
            => await Get(githubBranchFilter).FirstOrDefaultAsync();

        private IQueryable<GithubBranch> Get(GithubBranchFilter filter)
        {
            return _dbContext
                .Set<GithubBranch>()
                .Include(g => g.CodeSources)
                .WhereMatches(filter);
        }
        public async Task<GithubBranch> UpdateAsync(GithubBranch existingBranch)
        {
            _dbContext
                .Set<GithubBranch>()
                .Update(existingBranch);
            await _dbContext.SaveChangesAsync();
            return existingBranch;
        }

        public async Task CreateForNewSourceCodeAsync(CodeSource codeSource)
        {
            try
            {
                await _httpClientHelper.PostObjectResponseAsync<object>
                (
                    "createDefaultBranches",
                    new { codeSource.Id },
                    new Dictionary<string, string>()
                );
            }
            catch (RemoteServiceException e)
            {
                _logger.LogError(e, "Remote creation of default code source branches failed");
                throw;
            }
        }

    }

    internal static class GithubBranchesQueryableExtensions
    {
        public static IQueryable<GithubBranch> WhereMatches(this IQueryable<GithubBranch> githubBranches, GithubBranchFilter filter)
        {
            return githubBranches
                .WhenNotNullOrEmpty(filter.Name).ApplyWhere(gb => gb.Name == filter.Name)
                .WhenHasValue(filter.CodeSourceId).ApplyWhere(cs => cs.CodeSources.Any(c => c.Id == filter.CodeSourceId))
                .WhenHasValue(filter.IsDeleted).ApplyWhere(cs => cs.IsDeleted == filter.IsDeleted);
        }
    }
}
