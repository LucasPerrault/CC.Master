using System.Collections.Generic;

namespace Instances.Infra.CodeSources.Models
{
    public class RawJenkinsJob
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public List<RawJenkinsJob> Jobs { get; set; }
    }
}
