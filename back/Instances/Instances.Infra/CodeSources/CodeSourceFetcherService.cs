using Instances.Domain.CodeSources;
using Instances.Infra.Github;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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

        private readonly IGithubService _githubService;

        public CodeSourceFetcherService(IGithubService githubService)
        {
            _githubService = githubService;
        }

        public async Task<IEnumerable<CodeSource>> FetchAsync(string repoUrl)
        {
            var productionFileAsString = await _githubService.GetFileContentAsync(repoUrl, CodeSourceConfigFilePath);
            var productionFile = JsonConvert.DeserializeObject<ContinuousDeploymentProductionFile>(productionFileAsString);

            return CreateCodeSourcesFromFetchedApps(productionFile.Apps, repoUrl);
        }

        private IEnumerable<CodeSource> CreateCodeSourcesFromFetchedApps(IEnumerable<ProductionApp> apps, string repoUrl)
        {
            return apps.Select(app => new CodeSource
            {
                GithubRepo = repoUrl,
                Name = app.FriendlyName,
                Code = app.Name,
                Type = GetCodeSourceTypeFromProjectType(app.ProjectType),
                Lifecycle = CodeSourceLifecycleStep.Referenced,
                JenkinsProjectName = app.JenkinsProjectName,
                Config = new CodeSourceConfig()
                {
                    IsPrivate = app.IsPrivate,
                    IisServerPath = app.Path,
                    AppPath = app.AppPath,
                    Subdomain = app.WsTenant,
                }
            });
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
