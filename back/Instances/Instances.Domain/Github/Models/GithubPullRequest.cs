using System;

namespace Instances.Domain.Github.Models
{
    public class GithubPullRequest
    {
        public int Id { get; set; }
        public string Name => $"PR #{Number}";
        public int Number { get; set; }
        public string Title { get; set; }
        public bool IsOpened { get; set; }
        public DateTime OpenedAt { get; set; }
        public DateTime? MergedAt { get; set; }
        public DateTime? ClosedAt { get; set; }

        public int OriginBranchId { get; set; }
        public GithubBranch OriginBranch { get; set; }

        public int RepoId { get; set; }
        public GithubRepo Repo { get; set; }

    }
}
