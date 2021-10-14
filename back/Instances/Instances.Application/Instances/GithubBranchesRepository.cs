using Instances.Domain.CodeSources;
using Instances.Domain.CodeSources.Filtering;
using Instances.Domain.Github;
using Instances.Domain.Github.Models;
using Lucca.Core.Shared.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Instances.Application.Instances
{
    public class GithubBranchesRepository : IGithubBranchesRepository
    {
        private readonly IGithubBranchesStore _githubBranchesStore;
        private readonly IPreviewConfigurationsRepository _previewConfigurationsRepository;
        private readonly IGithubService _githubService;

        public GithubBranchesRepository(
            IGithubBranchesStore githubBranchesStore, IPreviewConfigurationsRepository previewConfigurationsRepository,
            IGithubService githubService)
        {
            _githubBranchesStore = githubBranchesStore;
            _githubService = githubService;
            _previewConfigurationsRepository = previewConfigurationsRepository;
        }

        public async Task<GithubBranch> CreateAsync(GithubBranch branch)
        {
            branch = await _githubBranchesStore.CreateAsync(branch);
            await _previewConfigurationsRepository.CreateByBranchAsync(branch);
            return branch;
        }

        public async Task<GithubBranch> CreateAsync(List<CodeSource> codeSources, string branchName, GithubApiCommit commit)
        {
            if (!codeSources.Any())
            {
                throw new ArgumentNullException(nameof(codeSources));
            }

            var repoUrl = codeSources.First().GithubRepo;
            var existingBranch = await GetNonDeletedBranchByNameAsync(codeSources.First(), branchName);
            if (existingBranch != null)
            {
                throw new BadRequestException($"Les sources de code du repo {repoUrl} contiennent déjà la branche {branchName}");
            }

            if (commit == null)
            {
                commit = await _githubService.GetGithubBranchHeadCommitInfoAsync(repoUrl, branchName);
            }


            var branch = await CreateAsync(new GithubBranch
            {
                Name = branchName,
                CodeSources = codeSources,
                CreatedAt = commit.CommitedOn,
                LastPushedAt = commit.CommitedOn,
                HeadCommitMessage = commit.Message,
                HeadCommitHash = commit.Sha,
            });
            return branch;
        }

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
