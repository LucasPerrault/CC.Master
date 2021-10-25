namespace Instances.Web.Controllers.Query
{
    public class HelmReleaseQuery
    {
        public string ReleaseName { get; init; }
        public string GitRef { get; init; }
        public bool LastStable { get; init; } = true;
    }
}
