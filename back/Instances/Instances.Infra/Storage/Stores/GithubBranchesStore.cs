using Instances.Domain.CodeSources;
using Instances.Domain.CodeSources.Filtering;
using Instances.Domain.Github;
using Instances.Domain.Github.Models;
using Microsoft.EntityFrameworkCore;
using Storage.Infra.Extensions;
using System;
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
                .Include(g => g.Repo)
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

        public Task<List<GithubBranch>> GetProductionBranchesAsync(GithubBranchFilter githubBranchFilter)
        {
            var codeSourceProductionVersions =
                _dbContext.Set<CodeSourceProductionVersion>()
                .Where(cspv =>
                    _dbContext.Set<CodeSourceProductionVersion>()
                        .GroupBy(pv => pv.CodeSource.RepoId)
                        .Select(kvp => kvp.Max(m => m.Id))
                        .Contains(cspv.Id)
                );

            return _dbContext.Set<GithubBranch>()
                .WhereMatches(githubBranchFilter)
                .Where(gb => codeSourceProductionVersions.Any(c => c.CodeSource.RepoId == gb.RepoId && c.BranchName == gb.Name))
                .ToListAsync();
        }
    }

    internal static class GithubBranchesQueryableExtensions
    {
        public static IQueryable<GithubBranch> WhereMatches(this IQueryable<GithubBranch> githubBranches, GithubBranchFilter filter)
        {
            return githubBranches
                .WhenNotNullOrEmpty(filter.Name).ApplyWhere(gb => gb.Name == filter.Name)
                .WhenNotNullOrEmpty(filter.HelmChart).ApplyWhere(gb => gb.HelmChart == filter.HelmChart)
                .Apply(filter.HasHelmChart).To(gb => gb.HelmChart != null)
                .WhenNotNullOrEmpty(filter.RepoIds).ApplyWhere(gb => filter.RepoIds.Contains(gb.RepoId))
                .WhenNotNullOrEmpty(filter.ExcludedRepoIds).ApplyWhere(gb => !filter.ExcludedRepoIds.Contains(gb.RepoId))
                .Apply(filter.IsDeleted).To(gb => gb.IsDeleted);
        }
    }
}
