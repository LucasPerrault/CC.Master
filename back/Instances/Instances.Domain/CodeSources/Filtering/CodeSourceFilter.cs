using System.Collections.Generic;

namespace Instances.Domain.CodeSources.Filtering
{
    public class CodeSourceFilter
    {
        public HashSet<int> Id { get; set; } = new HashSet<int>();
        public string Search { get; set; }
        public string Code { get; set; }
        public HashSet<CodeSourceLifecycleStep> Lifecycle { get; set; } = new HashSet<CodeSourceLifecycleStep>();

        public static CodeSourceFilter ById(int id) => new CodeSourceFilter { Id = new HashSet<int> { id } };
        public static CodeSourceFilter ActiveByCode(string code) => new CodeSourceFilter
        {
            Code = code,
            Lifecycle = CodeSource.ActiveSteps
        };
    }
}
