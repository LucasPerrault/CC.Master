using Instances.Application.Instances.Dtos;
using Instances.Domain.CodeSources;
using Instances.Domain.CodeSources.Filtering;
using Lucca.Core.Api.Abstractions.Paging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Instances.Application.CodeSources
{
    public interface ICodeSourcesRepository
    {
        Task<Page<CodeSource>> GetAsync(IPageToken pageToken, CodeSourceFilter codeSourceFilter);
        Task<CodeSource> CreateAsync(CreateCodeSourceDto codeSource);
        Task<CodeSource> UpdateAsync(int id, CodeSourceUpdate codeSourceUpdate);

        Task<IEnumerable<CodeSource>> FetchFromRepoAsync(Uri repoUrl);
        Task<CodeSource> GetByIdAsync(int id);
        Task UpdateProductionVersionAsync(CodeSourceProductionVersionDto dto);
        Task<string> GetBuildUrlAsync(string codeSourceCode, string branchName, string buildNumber);
        Task<List<CodeSourceArtifacts>> GetArtifactsAsync(int codeSourceId);
        Task<List<CodeSourceArtifacts>> GetInstanceCleaningArtifactsAsync();
        Task<List<CodeSourceArtifacts>> GetInstanceAnonymizationArtifactsAsync();
        Task<List<CodeSourceArtifacts>> GetInstancePreRestoreArtifactsAsync();
        Task<List<CodeSourceArtifacts>> GetInstancePostRestoreArtifactsAsync();
        Task<List<CodeSourceArtifacts>> GetMonolithArtifactsAsync();
    }
}
