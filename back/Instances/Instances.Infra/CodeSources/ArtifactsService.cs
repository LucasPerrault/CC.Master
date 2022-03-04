using Instances.Domain.CodeSources;
using Lucca.Core.Shared.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Instances.Infra.CodeSources
{
    public class JenkinsApiArtifactResult
    {
        public List<JenkinsApiArtifactElement> Artifacts { get; set; }
    }

    public class JenkinsApiArtifactElement
    {
        public string DisplayName { get; set; }
        public string FileName { get; set; }
        public string RelativePath { get; set; }
    }

    public class ArtifactsService : IArtifactsService
    {
        private readonly ILogger<ArtifactsService> _logger;
        private readonly ICodeSourceBuildUrlService _codeSourceBuildUrlService;
        private readonly HttpClient _httpClient;

        public ArtifactsService(
            ILogger<ArtifactsService> logger, ICodeSourceBuildUrlService codeSourceBuildUrlService,
            HttpClient httpClient)
        {
            _logger = logger;
            _codeSourceBuildUrlService = codeSourceBuildUrlService;
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<CodeSourceArtifacts>> GetArtifactsAsync(CodeSource source, string branchName, int buildNumber)
        {
            var jenkinsBaseUrl = await _codeSourceBuildUrlService.GenerateBuildUrlAsync(source, branchName, buildNumber.ToString());
            var listArtifactsUrlBuilder = $"{jenkinsBaseUrl}/api/json/?tree=artifacts[*]";

            var response = await _httpClient.GetAsync(listArtifactsUrlBuilder);
            if (!response.IsSuccessStatusCode)
            {
                if ((int)response.StatusCode >= 400 && (int)response.StatusCode < 500)
                {
                    _logger.LogWarning($"Artifacts not found for source {source.Id}, branch : {branchName}, buildNumber: {buildNumber}. Response code : {response.StatusCode}, body : {await response.Content.ReadAsStringAsync()}");
                    throw new BadRequestException($"Artifacts not found for source {source.Id}, branch : {branchName}, buildNumber: {buildNumber}.");
                }
                _logger.LogError($"Failed to get artifacts for codeSource {source.Id}, branch : {branchName}, buildNumber: {buildNumber}. Response code : {response.StatusCode}, body : {await response.Content.ReadAsStringAsync()}");
                throw new Exception($"Failed to get artifacts for codeSource {source.Id}, branch : {branchName}, buildNumber: {buildNumber}.");
            }

            await using var body = await response.Content.ReadAsStreamAsync();
            var result = await Tools.Serializer.DeserializeAsync<JenkinsApiArtifactResult>(body);

            return result.Artifacts.Select(artifact =>
                new CodeSourceArtifacts
                {
                    CodeSourceId = source.Id,
                    ArtifactUrl = $"{jenkinsBaseUrl}/artifact/{artifact.RelativePath}",
                    FileName = artifact.FileName,
                    ArtifactType = GetArtifactTypeFromFileName(artifact.FileName)
                }
            );
        }

        private CodeSourceArtifactType GetArtifactTypeFromFileName(string fileName) => fileName switch
        {
            string f when f.EndsWith("production.json") => CodeSourceArtifactType.ProductionJson,
            string f when f.EndsWith(".back.zip") => CodeSourceArtifactType.BackZip,
            string f when f.EndsWith(".front.zip") => CodeSourceArtifactType.FrontZip,
            string f when f.EndsWith("anonymization.sql") => CodeSourceArtifactType.AnonymizationScript,
            string f when f.EndsWith("clean.sql") => CodeSourceArtifactType.CleanScript,
            string f when f.EndsWith("prerestore.sql") => CodeSourceArtifactType.PreRestoreScript,
            string f when f.EndsWith("postrestore.sql") => CodeSourceArtifactType.PostRestoreScript,
            _ => CodeSourceArtifactType.Other
        };
    }
}
