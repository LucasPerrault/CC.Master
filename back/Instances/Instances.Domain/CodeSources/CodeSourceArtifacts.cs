using System;

namespace Instances.Domain.CodeSources
{
    public enum CodeSourceArtifactType
    {
        ProductionJson = 0,
        BackZip = 1,
        FrontZip = 2,
        CleanScript = 3,
        AnonymizationScript = 4,
        PreRestoreScript = 5,
        PostRestoreScript = 6,
        Other = 99
    }

    public class CodeSourceArtifacts
    {
        public int Id { get; set; }
        public int CodeSourceId { get; set; }
        public string FileName { get; set; }
        public Uri ArtifactUrl { get; set; }
        public CodeSourceArtifactType ArtifactType { get; set; }
    }
}
