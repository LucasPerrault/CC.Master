using System.Collections.Generic;

namespace Instances.Domain.CodeSources.Filtering
{
    public class CodeSourceFilter
    {
        public HashSet<int> Id { get; set; } = new HashSet<int>();
        public string Search { get; set; }
        public string Code { get; set; }
        public HashSet<CodeSourceLifecycleStep> Lifecycle { get; set; } = CodeSource.ActiveSteps;

        public static CodeSourceFilter ById(int id) => new CodeSourceFilter
        {
            Id = new HashSet<int> { id },
            Lifecycle = new HashSet<CodeSourceLifecycleStep>()
        };

        public static CodeSourceFilter ByCode(string code) => new CodeSourceFilter
        {
            Code = code,
            Lifecycle = new HashSet<CodeSourceLifecycleStep>()
        };
    }
}
