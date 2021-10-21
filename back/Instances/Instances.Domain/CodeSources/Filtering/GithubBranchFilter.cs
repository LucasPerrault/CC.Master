namespace Instances.Domain.CodeSources.Filtering
{
    public class GithubBranchFilter
    {
        public int? CodeSourceId { get; set; }
        public string Name { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
