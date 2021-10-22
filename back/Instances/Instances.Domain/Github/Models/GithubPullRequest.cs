using Instances.Domain.CodeSources;
using System;
using System.Collections.Generic;

namespace Instances.Domain.Github.Models
{
    public class GithubPullRequest
    {
        public int Id { get; set; }
        public string Name => $"PR #{Number}";
        public int Number { get; set; }
        public string Title { get; set; }
        public List<CodeSource> CodeSources { get; set; }
        public bool IsOpened { get; set; }
        public DateTime OpenedAt { get; set; }
        public DateTime? MergedAt { get; set; }
        public DateTime? ClosedAt { get; set; }

        public int OriginBranchId { get; set; }
        public GithubBranch OriginBranch { get; set; }

    }
}
