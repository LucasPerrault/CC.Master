using Instances.Domain.CodeSources;
using Instances.Domain.CodeSources.Filtering;
using Lucca.Core.Shared.Domain.Exceptions;
using System;
using System.Collections.Generic;
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

    public interface CodeSourceUpdate
    {
        public CodeSourceLifecycleStep Lifecycle { get; set; }
    }


    public class CodeSourcesAppController
    {
        private readonly ICodeSourcesStore _codeSourcesStore;
        private readonly ICodeSourceFetcherService _fetcherService;

        public CodeSourcesAppController
        (
            ICodeSourcesStore codeSourcesStore,
            ICodeSourceFetcherService fetcherService
        )
        {
            _codeSourcesStore = codeSourcesStore;
            _fetcherService = fetcherService;
        }

        public async Task<IEnumerable<CodeSource>> GetAsync(CodeSourceFilter filter)
        {
            return await _codeSourcesStore.GetAsync(filter);
        }

        public async Task<CodeSource> CreateAsync(CodeSource codeSource)
        {
            return await _codeSourcesStore.CreateAsync(codeSource);
        }

        public async Task<CodeSource> UpdateAsync(int id, CodeSourceUpdate codeSourceUpdate)
        {
            var codeSource = await _codeSourcesStore.GetByIdAsync(id);
            await _codeSourcesStore.UpdateLifecycleAsync(codeSource, codeSourceUpdate.Lifecycle);
            return codeSource;
        }

        public async Task<IEnumerable<CodeSource>> FetchFromRepoAsync(string repoUrl)
        {
            return await _fetcherService.FetchAsync(repoUrl);
        }

        public async Task UpdateProductionVersionAsync(CodeSourceProductionVersionDto dto)
        {
            var codeSource = await _codeSourcesStore.GetNonDeletedByCodeAsync(dto.CodeSourceCode);
            if (codeSource == null)
            {
                throw new BadRequestException($"Unknown code source with code {dto.CodeSourceCode}");
            }

            var productionVersion = new CodeSourceProductionVersion
            {
                BranchName = dto.BranchName,
                JenkinsBuildNumber = dto.JenkinsBuildNumber,
                CommitHash = dto.CommitHash,
                Date = DateTime.Now
            };

            await _codeSourcesStore.AddProductionVersionAsync(codeSource, productionVersion);
        }
    }
}
