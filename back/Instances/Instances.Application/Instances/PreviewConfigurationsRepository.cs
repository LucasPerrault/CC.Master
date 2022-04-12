using Instances.Domain.CodeSources;
using Instances.Domain.Github.Models;
using Instances.Domain.Preview;
using Instances.Domain.Preview.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Instances.Application.Instances
{
    public class PreviewConfigurationsRepository : IPreviewConfigurationsRepository
    {
        private readonly IPreviewConfigurationsStore _previewConfigurationsStore;

        public PreviewConfigurationsRepository(IPreviewConfigurationsStore previewConfigurationsStore)
        {
            _previewConfigurationsStore = previewConfigurationsStore;
        }

        public Task CreateByBranchAsync(IEnumerable<GithubBranch> branches, CodeSource codeSource)
        {
            var previewConfigurations = branches.Select(branch =>
                   CreatePreviewConfigurationObject(branch, codeSource, $"{codeSource.Name} - {branch.Name}")
            );
            return _previewConfigurationsStore.CreateAsync(previewConfigurations);
        }

        public Task CreateByBranchAsync(IEnumerable<GithubBranch> branches, IEnumerable<CodeSource> codeSources)
        {
            var previewConfigurations = branches.SelectMany(branch =>
                codeSources.Select(codeSource => CreatePreviewConfigurationObject(branch, codeSource, $"{codeSource.Name} - {branch.Name}"))
            );
            return _previewConfigurationsStore.CreateAsync(previewConfigurations);
        }

        public Task CreateByPullRequestAsync(GithubPullRequest pullRequest, GithubBranch originBranch, IEnumerable<CodeSource> codeSources)
        {
            var previewConfigurations = codeSources.Select(
                codeSource => CreatePreviewConfigurationObject(originBranch, codeSource, $"{codeSource.Name} - #{pullRequest.Number} : {pullRequest.Title}")
            );
            return _previewConfigurationsStore.CreateAsync(previewConfigurations);
        }

        public Task DeleteByBranchAsync(GithubBranch branch)
            => _previewConfigurationsStore.DeleteByBranchAsync(branch);

        private static PreviewConfiguration CreatePreviewConfigurationObject(GithubBranch branch, CodeSource codeSource, string name) => new()
        {
            Name = name,
            CreatedAt = DateTime.Now,
            IsDeleted = false,
            AutoUpdateType = AutoUpdateType.Never,
            CodeSourceMappings = new List<CodeSourceMapping>
            {
                new CodeSourceMapping { CodeSourceId = codeSource.Id, GithubBranchId = branch.Id}
            }
        };

    }
}
