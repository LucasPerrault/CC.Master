using Instances.Domain.CodeSources;
using Instances.Domain.Github.Models;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Instances.Domain.Preview.Models
{
    public class PreviewConfiguration
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public AutoUpdateType AutoUpdateType { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUsedAt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public List<CodeSourceMapping> CodeSourceMappings { get; set; }
    }

    public class CodeSourceMapping
    {
        public int Id { get; set; }
        [JsonIgnore]
        public string Name => $"{Id}";

        public int PreviewConfigurationId { get; set; }
        public int CodeSourceId { get; set; }
        public int GithubBranchId { get; set; }
        public string DeployedVersion { get; set; }

        public PreviewConfiguration PreviewConfiguration { get; set; }
        public CodeSource CodeSource { get; set; }
        public GithubBranch GithubBranch { get; set; }

    }
}
