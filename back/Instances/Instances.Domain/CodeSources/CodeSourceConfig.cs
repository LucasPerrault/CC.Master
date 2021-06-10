namespace Instances.Domain.CodeSources
{
    public class CodeSourceConfig
    {
        public int CodeSourceId { get; set; }
        public CodeSource CodeSource { get; set; }
        public string AppPath { get; set; }
        public string Subdomain { get; set; }
        public string IisServerPath { get; set; }
        public bool IsPrivate { get; set; }
    }
}
