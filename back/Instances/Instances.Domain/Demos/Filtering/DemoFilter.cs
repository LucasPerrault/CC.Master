using Tools;

namespace Instances.Domain.Demos.Filtering
{
    public class DemoFilter
    {
        public string Subdomain { get; set; }
        public string Search { get; set; }
        public BoolCombination IsActive { get; set; } = BoolCombination.Both;
        public BoolCombination IsTemplate { get; set; } = BoolCombination.Both;
        public BoolCombination IsProtected { get; set; } = BoolCombination.Both;

        public static DemoFilter Active() => new DemoFilter { IsActive = BoolCombination.TrueOnly };
    }
}
