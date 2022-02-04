using System.Collections.Generic;

namespace Instances.Domain.CodeSources.Filtering
{
    public class GithubPullRequestFilter
    {
        public int? Number { get; set; }
        public HashSet<int> RepoIds { get; set; } = new HashSet<int>();
    }
}
