using Instances.Application.Instances.Dtos;
using Instances.Domain.CodeSources;
using Instances.Domain.CodeSources.Filtering;
using Instances.Domain.Github;
using Instances.Domain.Github.Models;
using Lucca.Core.Shared.Domain.Exceptions;
using MoreLinq;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Instances.Application.Instances
{
    public class HelmRepository : IHelmRepository
    {
        private const string GithubOrganisationUrl = "https://github.com/LuccaSA/";

        private readonly ICodeSourcesStore _codeSourcesStore;
        private readonly IGithubBranchesStore _githubBranchesStore;

        public HelmRepository(ICodeSourcesStore codeSourcesStore, IGithubBranchesStore githubBranchesStore)
        {
            _codeSourcesStore = codeSourcesStore;
            _githubBranchesStore = githubBranchesStore;
        }

        public async Task CreateHelmAsync(string releaseName, string branchName, string helmChart)
        {
            var codeSources = await _codeSourcesStore.GetAsync(new CodeSourceFilter
            {
                GithubRepo = $"{GithubOrganisationUrl}{releaseName}",
                ExcludedLifecycle = new HashSet<CodeSourceLifecycleStep> { CodeSourceLifecycleStep.Deleted }
            });
            if (!codeSources.Any())
            {
                throw new BadRequestException($"Source code not found for repo {releaseName}");
            }

            var branches = await _githubBranchesStore.GetAsync(new GithubBranchFilter
            {
                Name = branchName,
                IsDeleted = false,
                CodeSourceId = codeSources.First().Id
            });

            foreach (var branch in branches)
            {
                branch.HelmChart = helmChart;
            }
            await _githubBranchesStore.UpdateAsync(branches);
        }

        public async Task<List<HelmRelease>> GetAllReleasesAsync(string releaseName, string gitRef, bool stable)
        {
            List<CodeSource> codeSources = null;

            if (!string.IsNullOrEmpty(releaseName))
            {
                codeSources = await _codeSourcesStore.GetAsync(new CodeSourceFilter
                {
                    GithubRepo = $"{GithubOrganisationUrl}{releaseName}",
                    ExcludedLifecycle = new HashSet<CodeSourceLifecycleStep> { CodeSourceLifecycleStep.Deleted }
                });
                if (!codeSources.Any())
                {
                    throw new BadRequestException($"Source code not found for repo {releaseName}");
                }
            }

            IEnumerable<GithubBranch> branches = await _githubBranchesStore.GetAsync(new GithubBranchFilter
            {
                IsDeleted = false,
                CodeSourceIds = codeSources?.Select(c => c.Id)?.ToList(),
                HasHelmChart = true,
                Name = gitRef
            });
            if (stable)
            {
                var stableBranches = (await _githubBranchesStore
                    .GetProductionBranchesAsync(codeSources))
                    .Select(d =>
                    {
                        d.Value.CodeSources = new List<CodeSource>
                        {
                            d.Key
                        };
                        return d.Value;
                    })
                    .Where(b => b.HelmChart != null);

                branches = stableBranches
                    .Concat(branches.ExceptBy(stableBranches, b => b.CodeSources.First().Id));
            }

            return branches
                .GroupBy(h => h.CodeSources.First().GithubRepo)
                .Select(kvp => kvp.OrderByDescending(v => v.Id).First())
                .Select(b => new HelmRelease
                {
                    GitRef = b.Name,
                    HelmChart = b.HelmChart,
                    ReleaseName = b.CodeSources.First().GithubRepo.Substring(GithubOrganisationUrl.Length)
                }).ToList();
        }
    }

    public class CodeSourceRepoComparer : IEqualityComparer<CodeSource>
    {
        public bool Equals(CodeSource x, CodeSource y)
            => x.GithubRepo == y.GithubRepo;

        public int GetHashCode([DisallowNull] CodeSource obj)
            => obj.GithubRepo.GetHashCode();
    }
}
