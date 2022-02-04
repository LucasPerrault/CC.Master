using System.Collections.Generic;

namespace Instances.Domain.CodeSources.Filtering
{
    public record GithubBranchFilter
    {
        public string Name { get; init; }
        public bool? IsDeleted { get; init; }
        public string HelmChart { get; init; }
        public bool? HasHelmChart { get; init; }
        public HashSet<int> RepoIds { get; init; } = new HashSet<int>();
        public HashSet<int> ExcludedRepoIds { get; init; } = new HashSet<int>();
    }
}
