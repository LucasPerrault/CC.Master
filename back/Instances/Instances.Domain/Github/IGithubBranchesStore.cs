using Instances.Domain.CodeSources;
using Instances.Domain.CodeSources.Filtering;
using Instances.Domain.Github.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Instances.Domain.Github
{
    public interface IGithubBranchesStore
    {
        Task<GithubBranch> CreateAsync(GithubBranch branch);
        Task<List<GithubBranch>> CreateAsync(IEnumerable<GithubBranch> branch);
        Task<GithubBranch> GetFirstAsync(GithubBranchFilter githubBranchFilter);
        Task<List<GithubBranch>> GetAsync(GithubBranchFilter githubBranchFilter);
        Task<List<GithubBranch>> GetProductionBranchesAsync(GithubBranchFilter githubBranchFilter);
        Task<GithubBranch> UpdateAsync(GithubBranch existingBranch);
        Task UpdateAsync(IEnumerable<GithubBranch> existingBranches);
    }
}
