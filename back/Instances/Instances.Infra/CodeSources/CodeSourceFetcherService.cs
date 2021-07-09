using Instances.Domain.CodeSources;
using Instances.Infra.CodeSources.Models;
using Instances.Infra.Github;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Instances.Infra.CodeSources
{
    internal class ContinuousDeploymentProductionFile
    {
        public List<ProductionApp> Apps { get; set; }
    }

    internal class ProductionApp
    {
        public string FriendlyName { get; set; }
        public string Name { get; set; }
        public string JenkinsProjectName { get; set; }
        public string ProjectType { get; set; }
        public string AppPath { get; set; }
        public string WsTenant { get; set; }
        public bool IsPrivate { get; set; }
        public string Path { get; set; }
    }

    public class CodeSourceFetcherService : ICodeSourceFetcherService
    {
        public const string CodeSourceConfigFilePath = ".cd/production.json";

        private readonly ILogger<CodeSourceFetcherService> _logger;
        private readonly IGithubService _githubService;
        private readonly HttpClient _httpClient;

        private List<RawJenkinsJob> _cacheRawJenkinsJobs = null;
        private SemaphoreSlim _lockLoadingJenkinsJobs = new SemaphoreSlim(1, 1);

        public CodeSourceFetcherService(
            IGithubService githubService, HttpClient httpClient,
            ILogger<CodeSourceFetcherService> logger)
        {
            _githubService = githubService;
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<IEnumerable<CodeSource>> FetchAsync(string repoUrl)
        {
            var productionFileAsString = await _githubService.GetFileContentAsync(repoUrl, CodeSourceConfigFilePath);
            var productionFile = JsonSerializer.Deserialize<ContinuousDeploymentProductionFile>(productionFileAsString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return await CreateCodeSourcesFromFetchedAppsAsync(productionFile.Apps, repoUrl);
        }

        private async Task<List<CodeSource>> CreateCodeSourcesFromFetchedAppsAsync(IEnumerable<ProductionApp> apps, string repoUrl)
        {
            var result = new List<CodeSource>();
            foreach (var app in apps)
            {
                result.Add(new CodeSource
                {
                    GithubRepo = repoUrl,
                    Name = app.FriendlyName,
                    Code = app.Name,
                    Type = GetCodeSourceTypeFromProjectType(app.ProjectType),
                    Lifecycle = CodeSourceLifecycleStep.Referenced,
                    JenkinsProjectName = app.JenkinsProjectName,
                    JenkinsProjectUrl = await GetJenkinsProjectUrlAsync(app),
                    Config = new CodeSourceConfig
                    {
                        IsPrivate = app.IsPrivate,
                        IisServerPath = app.Path,
                        AppPath = app.AppPath,
                        Subdomain = app.WsTenant,
                    }
                });
            }
            return result;
        }

        private async Task<string> GetJenkinsProjectUrlAsync(ProductionApp app)
        {
            var jobs = await GetAllJenkinsJobsAsync();

            return jobs?.FirstOrDefault(job => job.Name == app.JenkinsProjectName)?.Url;
        }

        private async Task<List<RawJenkinsJob>> GetAllJenkinsJobsAsync()
        {
            await _lockLoadingJenkinsJobs.WaitAsync();
            try
            {
                if (_cacheRawJenkinsJobs != null)
                {
                    return _cacheRawJenkinsJobs;
                }
                var response = await _httpClient.GetAsync("http://jenkins2.lucca.local:8080/api/json/?tree=jobs[name,url,jobs[name,url]]");
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Failed to get jenkins jobs, response {response.StatusCode} : {await response.Content.ReadAsStringAsync()} ");
                    return null;
                }
                await using var bodyStream = await response.Content.ReadAsStreamAsync();
                _cacheRawJenkinsJobs = (await JsonSerializer.DeserializeAsync<RawJenkinsJob>(bodyStream, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }))
                    .Jobs
                    .SelectMany(j => j.Jobs ?? new List<RawJenkinsJob>())
                    .ToList();
                return _cacheRawJenkinsJobs;
            }
            finally
            {
                _lockLoadingJenkinsJobs.Release();
            }
        }

        private CodeSourceType GetCodeSourceTypeFromProjectType(string projectType)
        {
            return projectType switch
            {
                "no-monolith" => CodeSourceType.App,
                "webservice" => CodeSourceType.WebService,
                "monolith" => CodeSourceType.Monolithe,
                "webservice-legacy" => CodeSourceType.WebServiceLegacy,
                "internalTool" => CodeSourceType.InternalTool,
                _ => throw new ApplicationException($"Unknown project type : {projectType}")
            };
        }
    }
}
