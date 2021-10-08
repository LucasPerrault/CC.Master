using Instances.Domain.CodeSources;
using Instances.Domain.CodeSources.Filtering;
using Instances.Domain.Github.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Instances.Domain.Github
{
    public interface IGithubBranchesStore
    {
        Task CreateForNewSourceCodeAsync(CodeSource codeSource);
        Task<GithubBranch> CreateAsync(GithubBranch branch);
        Task<GithubBranch> GetFirstAsync(GithubBranchFilter githubBranchFilter);
        Task<GithubBranch> UpdateAsync(GithubBranch existingBranch);
    }
}
