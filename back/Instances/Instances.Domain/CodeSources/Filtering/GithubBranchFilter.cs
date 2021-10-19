using System.Collections.Generic;

namespace Instances.Domain.CodeSources.Filtering
{
    public class GithubBranchFilter
    {
        public int? CodeSourceId { get; set; }
        public List<int> CodeSourceIds { get; set; }
        public string Name { get; set; }
        public bool? IsDeleted { get; set; }
        public string HelmChart { get; set; }
        public bool? HasHelmChart { get; set; }
    }
}
