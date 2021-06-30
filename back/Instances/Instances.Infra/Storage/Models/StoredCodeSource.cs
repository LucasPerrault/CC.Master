using Instances.Domain.CodeSources;
using System.Collections.Generic;
using System.Linq;

namespace Instances.Infra.Storage.Models
{
    public class StoredCodeSource
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string JenkinsProjectName { get; set; }
        public CodeSourceType Type { get; set; }
        public string GithubRepo { get; set; }
        public CodeSourceLifecycleStep Lifecycle { get; set; }
        public CodeSourceConfig Config { get; set; }
        public List<CodeSourceProductionVersion> ProductionVersions { get; set; }

        public CodeSource ToCodeSource()
        {
            return new CodeSource
            {
                Id = Id,
                Name = Name,
                Code = Code,
                Type = Type,
                Config = Config,
                Lifecycle = Lifecycle,
                GithubRepo = GithubRepo,
                JenkinsProjectName = JenkinsProjectName,
                CurrentProductionVersion = ProductionVersions.OrderByDescending(v => v.Id).FirstOrDefault()
            };
        }

        public static StoredCodeSource FromCodeSource(CodeSource codeSource) => new StoredCodeSource
            {
                Id = codeSource.Id,
                Name = codeSource.Name,
                Code = codeSource.Code,
                Type = codeSource.Type,
                Config = codeSource.Config,
                Lifecycle = codeSource.Lifecycle,
                GithubRepo = codeSource.GithubRepo,
                JenkinsProjectName = codeSource.JenkinsProjectName,
                ProductionVersions = new List<CodeSourceProductionVersion>()
            };
    }

}
