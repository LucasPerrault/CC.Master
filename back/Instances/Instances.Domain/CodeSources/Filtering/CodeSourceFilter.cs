using System.Collections.Generic;

namespace Instances.Domain.CodeSources.Filtering
{
    public class CodeSourceFilter
    {
        public HashSet<int> Id { get; set; } = new HashSet<int>();
        public string Search { get; set; }
        public string Code { get; set; }
        public HashSet<int> RepoIds { get; set; } = new HashSet<int>();
        public HashSet<CodeSourceLifecycleStep> Lifecycle { get; set; } = new HashSet<CodeSourceLifecycleStep>();
        public HashSet<CodeSourceLifecycleStep> ExcludedLifecycle { get; set; } = new HashSet<CodeSourceLifecycleStep>();
        public HashSet<CodeSourceType> Type { get; set; } = new HashSet<CodeSourceType>();
        public string RepoName { get; set; }

        public static CodeSourceFilter ById(int id) => new CodeSourceFilter { Id = new HashSet<int> { id } };
        public static CodeSourceFilter ActiveByCode(string code) => new CodeSourceFilter
        {
            Code = code,
            Lifecycle = CodeSource.ActiveSteps
        };
        public static CodeSourceFilter MonotenantInProduction => new CodeSourceFilter
        {
            Type = CodeSource.MonotenantTypes,
            Lifecycle = new HashSet<CodeSourceLifecycleStep> { CodeSourceLifecycleStep.InProduction },
        };
    }
}
