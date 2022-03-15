using System.Collections.Generic;
using Tools;

namespace Instances.Domain.CodeSources.Filtering
{
    public record GithubBranchFilter
    {
        public string Name { get; init; }
        public CompareBoolean IsDeleted { get; init; } = CompareBoolean.Bypass;
        public string HelmChart { get; init; }
        public CompareBoolean HasHelmChart { get; init; } = CompareBoolean.Bypass;
        public HashSet<int> RepoIds { get; init; } = new HashSet<int>();
        public HashSet<int> ExcludedRepoIds { get; init; } = new HashSet<int>();
    }
}
