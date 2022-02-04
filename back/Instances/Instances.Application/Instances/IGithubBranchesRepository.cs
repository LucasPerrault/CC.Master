using Instances.Domain.Github.Models;
using System.Threading.Tasks;

namespace Instances.Application.Instances
{
    public interface IGithubBranchesRepository
    {
        Task<GithubBranch> GetNonDeletedBranchByNameAsync(int repoId, string branchName);
        Task<GithubBranch> CreateAsync(int repoId, string branchName, GithubApiCommit commit = null);
        Task<GithubBranch> UpdateAsync(GithubBranch existingBranch);
    }
}
