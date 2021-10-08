using Instances.Domain.CodeSources;
using System;
using System.Collections.Generic;
using Tools;

namespace Instances.Domain.Github.Models
{
    public class GithubBranch
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<CodeSource> CodeSources { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastPushedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string HeadCommitHash { get; set; }
        public string HeadCommitMessage { get; set; }

        public static string NormalizeName(string branchName)
            => branchName.Truncate(255);
    }
}
