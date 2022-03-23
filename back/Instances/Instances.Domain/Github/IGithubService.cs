using Instances.Domain.Github.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Instances.Domain.Github
{
    public interface IGithubService
    {
        Task<string> GetFileContentAsync(Uri repoUrl, string filepath);
        Task<GithubApiCommit> GetGithubBranchHeadCommitInfoAsync(Uri repoUrl, string branchName);
        Task<IEnumerable<string>> GetBranchNamesAsync(Uri repoUrl);
    }
}
