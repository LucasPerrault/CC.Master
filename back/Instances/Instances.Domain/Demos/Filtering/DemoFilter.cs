using Tools;

namespace Instances.Domain.Demos.Filtering
{
    public class DemoFilter
    {
        public string Subdomain { get; set; }
        public BoolCombination IsActive { get; set; } = BoolCombination.Both;

        public static DemoFilter Active() => new DemoFilter { IsActive = BoolCombination.TrueOnly };
    }
}
