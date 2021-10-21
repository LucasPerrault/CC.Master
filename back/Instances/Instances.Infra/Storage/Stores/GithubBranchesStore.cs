using Instances.Domain.CodeSources.Filtering;
using Instances.Domain.Github;
using Instances.Domain.Github.Models;
using Microsoft.EntityFrameworkCore;
using Storage.Infra.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Instances.Infra.Storage.Stores
{
    public class GithubBranchesStore : IGithubBranchesStore
    {
        private readonly InstancesDbContext _dbContext;

        public GithubBranchesStore(InstancesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<GithubBranch> CreateAsync(GithubBranch branch)
        {
            _dbContext.Set<GithubBranch>().Add(branch);
            await _dbContext.SaveChangesAsync();
            return branch;
        }

        public async Task<List<GithubBranch>> CreateAsync(IEnumerable<GithubBranch> branches)
        {
            _dbContext.Set<GithubBranch>().AddRange(branches);
            await _dbContext.SaveChangesAsync();
            return branches.ToList();
        }

        public async Task<GithubBranch> GetFirstAsync(GithubBranchFilter githubBranchFilter)
            => await Get(githubBranchFilter).FirstOrDefaultAsync();

        public async Task<List<GithubBranch>> GetAsync(GithubBranchFilter githubBranchFilter)
            => await Get(githubBranchFilter).ToListAsync();


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
        public async Task UpdateAsync(IEnumerable<GithubBranch> existingBranches)
        {
            _dbContext
                .Set<GithubBranch>()
                .UpdateRange(existingBranches);
            await _dbContext.SaveChangesAsync();
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
