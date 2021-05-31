using Tools;

namespace Environments.Domain.Storage
{
    public class EnvironmentFilter
    {
        public CompareString Subdomain { get; set; } = CompareString.MatchAll;
        public BoolCombination IsActive { get; set; } = BoolCombination.Both;
    }
}
