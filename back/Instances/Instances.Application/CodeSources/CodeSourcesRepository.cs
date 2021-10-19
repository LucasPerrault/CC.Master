using Instances.Application.Instances;
using Instances.Domain.CodeSources;
using Instances.Domain.CodeSources.Filtering;
using Instances.Domain.Github;
using Instances.Domain.Github.Models;
using Instances.Domain.Preview;
using Instances.Domain.Preview.Models;
using Lucca.Core.Api.Abstractions.Paging;
using Lucca.Core.Shared.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Instances.Application.CodeSources
{
    public class CodeSourceProductionVersionDto
    {
        public string CodeSourceCode { get; set; }
        public string BranchName { get; set; }
        public int JenkinsBuildNumber { get; set; }
        public string CommitHash { get; set; }
    }

    public class CodeSourceUpdate
    {
        public CodeSourceLifecycleStep Lifecycle { get; set; }
    }


    public class CodeSourcesRepository : ICodeSourcesRepository
    {
        private readonly ICodeSourcesStore _codeSourcesStore;
        private readonly IGithubBranchesStore _githubBranchesStore;
        private readonly ICodeSourceFetcherService _fetcherService;
        private readonly ICodeSourceBuildUrlService _codeSourceBuildUrl;
        private readonly IArtifactsService _artifactsService;
        private readonly IGithubService _githubService;
        private readonly IPreviewConfigurationsRepository _previewConfigurationsRepository;

        public CodeSourcesRepository
        (
            ICodeSourcesStore codeSourcesStore, IGithubBranchesStore githubBranchesStore,
            ICodeSourceFetcherService fetcherService, ICodeSourceBuildUrlService codeSourceBuildUrl,
            IArtifactsService artifactsService, IGithubService githubService,
            IPreviewConfigurationsRepository previewConfigurationsRepository
        )
        {
            _codeSourcesStore = codeSourcesStore;
            _githubBranchesStore = githubBranchesStore;
            _fetcherService = fetcherService;
            _codeSourceBuildUrl = codeSourceBuildUrl;
            _artifactsService = artifactsService;
            _githubService = githubService;
            _previewConfigurationsRepository = previewConfigurationsRepository;
        }

        public async Task<Page<CodeSource>> GetAsync(IPageToken pageToken, CodeSourceFilter codeSourceFilter)
        {
            return await _codeSourcesStore.GetAsync(pageToken, codeSourceFilter);
        }

        public async Task<CodeSource> CreateAsync(CodeSource codeSource)
        {
            var otherCodeSourceIds = await _codeSourcesStore.GetAsync(new CodeSourceFilter
            {
                GithubRepo = codeSource.GithubRepo
            });

            codeSource = await _codeSourcesStore.CreateAsync(codeSource);

            if (otherCodeSourceIds.Any())
            {
                var branchesToUpdate = await _githubBranchesStore.GetAsync(new GithubBranchFilter
                {
                    CodeSourceId = otherCodeSourceIds.First().Id,
                    IsDeleted = false
                });

                foreach (var branch in branchesToUpdate)
                {
                    branch.CodeSources.Add(codeSource);
                }
                await _githubBranchesStore.UpdateAsync(branchesToUpdate);
                return codeSource;
            }

            var branchNames = await _githubService.GetBranchNamesAsync(codeSource.GithubRepo);
            var githubBranches = new List<GithubBranch>(capacity: branchNames.Count());
            foreach (var branchName in branchNames)
            {
                var headCommitInfo = await _githubService.GetGithubBranchHeadCommitInfoAsync(codeSource.GithubRepo, branchName);
                githubBranches.Add(new GithubBranch
                {
                    Name = branchName,
                    CodeSources = new() { codeSource },
                    CreatedAt = headCommitInfo.CommitedOn,
                    LastPushedAt = headCommitInfo.CommitedOn,
                    HeadCommitMessage = headCommitInfo.Message,
                    HeadCommitHash = headCommitInfo.Sha,
                });
            }
            githubBranches = await _githubBranchesStore.CreateAsync(githubBranches);
            await _previewConfigurationsRepository.CreateByBranchAsync(githubBranches);

            return codeSource;
        }

        public async Task<CodeSource> UpdateAsync(int id, CodeSourceUpdate codeSourceUpdate)
        {
            var source = await GetSingleOrDefaultAsync(CodeSourceFilter.ById(id));
            await _codeSourcesStore.UpdateLifecycleAsync(source, codeSourceUpdate.Lifecycle);
            return source;
        }

        public async Task<List<CodeSource>> GetNonDeletedByRepositoryUrlAsync(string repositoryUrl)
        {
            return await _codeSourcesStore.GetAsync(
                new CodeSourceFilter
                {
                    GithubRepo = repositoryUrl,
                    ExcludedLifecycle = new HashSet<CodeSourceLifecycleStep> { CodeSourceLifecycleStep.Deleted }
                }
            );
        }

        public async Task<IEnumerable<CodeSource>> FetchFromRepoAsync(string repoUrl)
        {
            return await _fetcherService.FetchAsync(repoUrl);
        }

        public async Task<CodeSource> GetByIdAsync(int id)
        {
            return await GetSingleOrDefaultAsync(CodeSourceFilter.ById(id));
        }

        public async Task UpdateProductionVersionAsync(CodeSourceProductionVersionDto dto)
        {
            var source = await GetSingleOrDefaultAsync(CodeSourceFilter.ActiveByCode(dto.CodeSourceCode));

            var productionVersion = new CodeSourceProductionVersion
            {
                BranchName = dto.BranchName,
                JenkinsBuildNumber = dto.JenkinsBuildNumber,
                CommitHash = dto.CommitHash,
                Date = DateTime.Now
            };

            var codeSourceArtifacts = await _artifactsService.GetArtifactsAsync(source, dto.BranchName, dto.JenkinsBuildNumber);
            await _codeSourcesStore.ReplaceProductionArtifactsAsync(source, codeSourceArtifacts);

            await _codeSourcesStore.AddProductionVersionAsync(source, productionVersion);
            if (source.Lifecycle != CodeSourceLifecycleStep.InProduction)
            {
                await _codeSourcesStore.UpdateLifecycleAsync(source, CodeSourceLifecycleStep.InProduction);
            }
        }

        public async Task<string> GetBuildUrlAsync(string codeSourceCode, string branchName, string buildNumber)
        {
            if (!_codeSourceBuildUrl.IsValidBuildNumber(buildNumber))
            {
                throw new BadRequestException("Build number is invalid");
            }

            var codeSource = await GetSingleOrDefaultAsync(CodeSourceFilter.ActiveByCode(codeSourceCode));
            if (string.IsNullOrEmpty(codeSource.JenkinsProjectUrl))
            {
                throw new BadRequestException("Build url not found for this code source");
            }
            return await _codeSourceBuildUrl.GenerateBuildUrlAsync(codeSource, branchName, buildNumber);
        }

        public Task<List<CodeSourceArtifacts>> GetArtifactsAsync(int codeSourceId)
        {
            return _codeSourcesStore.GetArtifactsAsync(codeSourceId);
        }

        private async Task<CodeSource> GetSingleOrDefaultAsync(CodeSourceFilter filter)
        {
            var codeSources = await _codeSourcesStore.GetAsync(filter);
            var source = codeSources.SingleOrDefault();
            if (source == null)
            {
                throw new NotFoundException("Unknown code source");
            }

            return source;
        }
    }
}
