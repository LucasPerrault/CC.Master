using Instances.Domain.CodeSources;
using Instances.Domain.CodeSources.Filtering;
using Instances.Domain.Github;
using Lucca.Core.Shared.Domain.Exceptions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Instances.Application.Instances
{
    public class HelmRepository : IHelmRepository
    {
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
    }
}
