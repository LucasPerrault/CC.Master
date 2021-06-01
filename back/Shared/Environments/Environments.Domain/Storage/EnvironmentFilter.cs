using Tools;

namespace Environments.Domain.Storage
{
    public class EnvironmentFilter
    {
        public CompareString Subdomain { get; set; } = CompareString.Bypass;
        public CompareBoolean IsActive { get; set; } = CompareBoolean.Bypass;
        public string Search { get; set; }
    }
}
