using System.Collections.Generic;

namespace Instances.Domain.CodeSources.Filtering
{
    public class CodeSourceFilter
    {
        public List<CodeSourceLifecycleStep> Lifecycle { get; set; }
        public string Search { get; set; }
    }
}
