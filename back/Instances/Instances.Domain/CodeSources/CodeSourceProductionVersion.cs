using System;
using System.Text.Json.Serialization;

namespace Instances.Domain.CodeSources
{
    public class CodeSourceProductionVersion
    {
        public int Id { get; set; }
        public string Name => $"{BranchName} #{JenkinsBuildNumber} ({CommitHash})";
        public string BranchName { get; set; }
        public int JenkinsBuildNumber { get; set; }
        public string CommitHash { get; set; }
        public DateTime Date { get; set; }
        [JsonIgnore]
        public CodeSource CodeSource { get; set; }
        public int CodeSourceId { get; set; }
    }
}
