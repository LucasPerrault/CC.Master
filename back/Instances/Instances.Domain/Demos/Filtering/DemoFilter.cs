using Tools;

namespace Instances.Domain.Demos.Filtering
{
    public class DemoFilter
    {
        public CompareString Subdomain { get; set; } = CompareString.MatchAll;
        public string Search { get; set; }
        public BoolCombination IsActive { get; set; } = BoolCombination.Both;
        public BoolCombination IsTemplate { get; set; } = BoolCombination.Both;
        public BoolCombination IsProtected { get; set; } = BoolCombination.Both;
        public string DistributorId { get; set; }
        public int? AuthorId { get; set; }

        public static DemoFilter Active() => new DemoFilter { IsActive = BoolCombination.TrueOnly };
    }
}
