using Instances.Domain.Github.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Instances.Domain.CodeSources
{

    public enum CodeSourceType
    {
        App = 0,
        WebService = 1,
        Monolithe = 2,
        WebServiceLegacy = 3,
        InternalTool = 4,
    }

    public enum CodeSourceLifecycleStep
    {
        Referenced = 0,
        Preview = 1,
        ReadyForDeploy = 2,
        InProduction = 3,
        ToDelete = 4,
        Deleted = 5,
    }

    public class CodeSource
    {

        public static HashSet<CodeSourceLifecycleStep> ActiveSteps => Enum
            .GetValues(typeof(CodeSourceLifecycleStep))
            .Cast<CodeSourceLifecycleStep>()
            .Except(new[] { CodeSourceLifecycleStep.Deleted, CodeSourceLifecycleStep.ToDelete })
            .ToHashSet();

        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string JenkinsProjectName { get; set; }
        public string JenkinsProjectUrl { get; set; }
        public CodeSourceType Type { get; set; }
        public string GithubRepo { get; set; }
        public CodeSourceLifecycleStep Lifecycle { get; set; }
        public CodeSourceConfig Config { get; set; }

        public CodeSourceProductionVersion CurrentProductionVersion => ProductionVersions.OrderByDescending(v => v.Id).FirstOrDefault();

        public List<CodeSourceArtifacts> CodeSourceArtifacts { get; set; }
        public List<CodeSourceProductionVersion> ProductionVersions { get; set; }
        public List<GithubBranch> GithubBranches { get; set; }
        public List<GithubPullRequest> GithubPullRequests { get; set; }
    }
}
