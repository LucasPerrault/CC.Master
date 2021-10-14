using Instances.Domain.CodeSources;
using Instances.Domain.Github.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Instances.Application.Instances
{
    public interface IGithubBranchesRepository
    {
        Task<GithubBranch> GetNonDeletedBranchByNameAsync(CodeSource firstCodeSource, string branchName);
        Task<GithubBranch> CreateAsync(GithubBranch branch);
        Task<GithubBranch> CreateAsync(List<CodeSource> codeSources, string branchName, GithubApiCommit commit = null);
        Task<GithubBranch> UpdateAsync(GithubBranch existingBranch);
    }
}
