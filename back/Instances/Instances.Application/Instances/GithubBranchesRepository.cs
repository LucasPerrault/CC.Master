using Instances.Domain.CodeSources;
using Instances.Domain.CodeSources.Filtering;
using Instances.Domain.Github;
using Instances.Domain.Github.Models;
using System.Threading.Tasks;

namespace Instances.Application.Instances
{
    public class GithubBranchesRepository : IGithubBranchesRepository
    {
        private readonly IGithubBranchesStore _githubBranchesStore;

        public GithubBranchesRepository(IGithubBranchesStore githubBranchesStore)
        {
            _githubBranchesStore = githubBranchesStore;
        }

        public Task<GithubBranch> CreateAsync(GithubBranch branch)
            => _githubBranchesStore.CreateAsync(branch);

        public Task<GithubBranch> GetNonDeletedBranchByNameAsync(CodeSource firstCodeSource, string branchName)
            => _githubBranchesStore.GetFirstAsync(new GithubBranchFilter
            {
                CodeSourceId = firstCodeSource.Id,
                Name = GithubBranch.NormalizeName(branchName),
                IsDeleted = false
            });

        public Task<GithubBranch> UpdateAsync(GithubBranch existingBranch)
            => _githubBranchesStore.UpdateAsync(existingBranch);
    }
}
