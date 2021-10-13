using Instances.Domain.Github.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Instances.Domain.Github
{
    public interface IGithubService
    {
        Task<string> GetFileContentAsync(string repoUrl, string filepath);
        Task<GithubApiCommit> GetGithubBranchHeadCommitInfoAsync(string githubRepo, string branchName);
        Task<IEnumerable<string>> GetBranchNamesAsync(string githubRepo);
    }
}
