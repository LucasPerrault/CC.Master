using Instances.Domain.CodeSources;
using Instances.Domain.CodeSources.Filtering;
using Instances.Domain.Github;
using Instances.Domain.Github.Models;
using Lucca.Core.Shared.Domain.Exceptions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Instances.Application.Instances
{
    public class GithubBranchesRepository : IGithubBranchesRepository
    {
        private readonly IGithubBranchesStore _githubBranchesStore;
        private readonly IGithubReposStore _githubReposStore;
        private readonly IPreviewConfigurationsRepository _previewConfigurationsRepository;
        private readonly IGithubService _githubService;
        private readonly ICodeSourcesStore _codeSourcesStore;

        public GithubBranchesRepository
        (
            IGithubBranchesStore githubBranchesStore,
            IGithubReposStore githubReposStore,
            IPreviewConfigurationsRepository previewConfigurationsRepository,
            IGithubService githubService,
            ICodeSourcesStore codeSourcesStore
        )
        {
            _githubBranchesStore = githubBranchesStore;
            _githubReposStore = githubReposStore;
            _githubService = githubService;
            _previewConfigurationsRepository = previewConfigurationsRepository;
            _codeSourcesStore = codeSourcesStore;
        }

        public async Task<GithubBranch> CreateAsync(int repoId, string branchName, GithubApiCommit commit)
        {
            var repo = await _githubReposStore.GetByIdAsync(repoId);
            var existingBranch = await GetNonDeletedBranchByNameAsync(repoId, branchName);
            if (existingBranch != null)
            {
                throw new BadRequestException($"Les sources de code du repo {repoId} contiennent déjà la branche {branchName}");
            }

            if (commit == null)
            {
                commit = await _githubService.GetGithubBranchHeadCommitInfoAsync(repo.Url, branchName);
            }

            var branch = await CreateAsync(new GithubBranch
            {
                Name = branchName,
                RepoId = repoId,
                CreatedAt = commit.CommitedOn,
                LastPushedAt = commit.CommitedOn,
                HeadCommitMessage = commit.Message,
                HeadCommitHash = commit.Sha,
                Repo = repo
            });
            return branch;
        }

        public Task<GithubBranch> GetNonDeletedBranchByNameAsync(int repoId, string branchName)
            => _githubBranchesStore.GetFirstAsync(new GithubBranchFilter
            {
                RepoIds = new HashSet<int> { repoId },
                Name = GithubBranch.NormalizeName(branchName),
                IsDeleted = Tools.CompareBoolean.FalseOnly,
            });

        public Task<GithubBranch> UpdateAsync(GithubBranch existingBranch)
            => _githubBranchesStore.UpdateAsync(existingBranch);

        private async Task<GithubBranch> CreateAsync(GithubBranch branch)
        {
            var createdBranch = await _githubBranchesStore.CreateAsync(branch);
            var codeSources = await _codeSourcesStore.GetAsync(new CodeSourceFilter
            {
                RepoIds = new HashSet<int> { createdBranch.RepoId },
                ExcludedLifecycle = new HashSet<CodeSourceLifecycleStep> { CodeSourceLifecycleStep.ToDelete, CodeSourceLifecycleStep.Deleted },
            });
            await _previewConfigurationsRepository.CreateByBranchAsync(new[] { createdBranch }, codeSources);
            return createdBranch;
        }
    }
}
