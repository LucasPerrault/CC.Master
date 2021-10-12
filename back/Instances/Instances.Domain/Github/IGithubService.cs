using Instances.Domain.Github.Models;
using System.Threading.Tasks;

namespace Instances.Domain.Github
{
    public interface IGithubService
    {
        Task<string> GetFileContentAsync(string repoUrl, string filepath);
        Task<GithubCommit> GetGithubBranchHeadCommitInfoAsync(string githubRepo, string branchName);
    }
}
