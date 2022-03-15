using Instances.Application.Instances.Dtos;
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
    public class HelmRepository : IHelmRepository
    {
        private const string GithubOrganisationUrl = "https://github.com/LuccaSA/";

        private readonly IGithubBranchesStore _githubBranchesStore;
        private readonly IGithubReposStore _githubReposStore;

        public HelmRepository(IGithubBranchesStore githubBranchesStore, IGithubReposStore githubReposStore)
        {
            _githubBranchesStore = githubBranchesStore;
            _githubReposStore = githubReposStore;
        }

        public async Task CreateHelmAsync(string releaseName, string branchName, string helmChart)
        {
            var repo = await GetRepoByReleaseNameAsync(releaseName);

            var branches = await _githubBranchesStore.GetAsync(new GithubBranchFilter
            {
                RepoIds = new HashSet<int> { repo.Id },
                Name = branchName,
                IsDeleted = Tools.CompareBoolean.FalseOnly,
            });

            foreach (var branch in branches)
            {
                branch.HelmChart = helmChart;
            }
            await _githubBranchesStore.UpdateAsync(branches);
        }

        private async Task<GithubRepo> GetRepoByReleaseNameAsync(string releaseName)
        {
            var repoUri = new Uri(new Uri(GithubOrganisationUrl), releaseName);
            var repo = await _githubReposStore.GetByUriAsync(repoUri);
            if (repo is null)
            {
                throw new BadRequestException($"Repo {repoUri} not found");
            }

            return repo;
        }

        private class HelmReleaseBranch
        {
            public GithubBranch Branch { get; set; }
            public bool IsProd { get; set; }
        }

        public async Task<List<HelmRelease>> GetAllReleasesAsync(HelmRequest helmRequest)
        {
            var repoIdsFilter = new HashSet<int>();
            var githubBranchFilter = new GithubBranchFilter { RepoIds = repoIdsFilter, HasHelmChart = Tools.CompareBoolean.TrueOnly, IsDeleted = Tools.CompareBoolean.FalseOnly };
            if (helmRequest is SpecificRepoHelmRequest specificRepoHelmRequest)
            {
                var repo = await GetRepoByReleaseNameAsync(specificRepoHelmRequest.RepoName);
                githubBranchFilter = githubBranchFilter with
                {
                    RepoIds = new HashSet<int> { repo.Id },
                    Name = specificRepoHelmRequest.GitRef
                };
            }

            List<HelmReleaseBranch> stableHelmReleasesBranches;
            if (helmRequest.ShouldBeStable)
            {
                var productionBranches = await _githubBranchesStore.GetProductionBranchesAsync(githubBranchFilter);
                stableHelmReleasesBranches = productionBranches
                    .Where(pb => pb.HelmChart != null)
                    .Select
                    (
                        b => new HelmReleaseBranch
                        {
                            Branch = b,
                            IsProd = true,
                        }
                    ).ToList();
            }
            else
            {
                stableHelmReleasesBranches = new List<HelmReleaseBranch>();
            }

            var missingHelmBranches = helmRequest.ShouldBeStable
                ? await GetMissingStableBranchesAsync(stableHelmReleasesBranches, githubBranchFilter)
                : await GetAllMissingBranchesAsync(stableHelmReleasesBranches, githubBranchFilter);

            return stableHelmReleasesBranches.Concat
                (
                    missingHelmBranches.Select
                    (
                        b => new HelmReleaseBranch
                        {
                            Branch = b,
                            IsProd = false,
                        }
                    )
                )
                .Select(b => new HelmRelease
                {
                    GitRef = b.Branch.Name,
                    HelmChart = b.Branch.HelmChart,
                    ReleaseName = b.Branch.Repo.Url.ToString().Substring(GithubOrganisationUrl.Length),
                    IsProductionVersion = b.IsProd,
                })
                .ToList();
        }

        private async Task<IEnumerable<GithubBranch>> GetAllMissingBranchesAsync(List<HelmReleaseBranch> alreadyCreated, GithubBranchFilter filter)
        {
            var branches = await _githubBranchesStore.GetAsync(filter);
            return branches
                .ExceptBy(alreadyCreated.Select(b => b.Branch.Id), b => b.Id);
        }

        private async Task<IEnumerable<GithubBranch>> GetMissingStableBranchesAsync(List<HelmReleaseBranch> alreadyCreated, GithubBranchFilter githubBranchFilter)
        {
            var repos = await _githubReposStore.GetAllAsync();

            var alreadyHandledRepoIds = alreadyCreated
                .Select(b => b.Branch.RepoId).ToHashSet();

            var missingBranchesFilter = githubBranchFilter with
            {
                ExcludedRepoIds = alreadyHandledRepoIds,
                HasHelmChart = Tools.CompareBoolean.TrueOnly,
                IsDeleted = Tools.CompareBoolean.FalseOnly
            };
            var missingBranchesCandidates = await _githubBranchesStore.GetAsync(missingBranchesFilter);

            return missingBranchesCandidates
                .GroupBy(b => b.RepoId)
                .Select(g => g.OrderByDescending(b => b.LastPushedAt ?? b.CreatedAt).First());
        }
    }
}
