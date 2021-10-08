using Instances.Domain.CodeSources;
using Instances.Domain.Github.Models;
using System.Threading.Tasks;

namespace Instances.Application.Instances
{
    public interface IGithubBranchesRepository
    {
        Task<GithubBranch> GetNonDeletedBranchByNameAsync(CodeSource firstCodeSource, string branchName);
        Task<GithubBranch> CreateAsync(GithubBranch branch);
        Task<GithubBranch> UpdateAsync(GithubBranch existingBranch);
    }
}
