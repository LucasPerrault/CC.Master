namespace Instances.Application.Instances.Dtos
{
    public class HelmRelease
    {
        public string ReleaseName { get; init; }
        public string HelmChart { get; init; }
        public string GitRef { get; init; }
    }
}
