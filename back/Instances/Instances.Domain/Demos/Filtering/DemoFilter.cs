using Tools;

namespace Instances.Domain.Demos.Filtering
{
    public class DemoFilter
    {
        public string Subdomain { get; set; }
        public BoolCombination IsActive { get; set; } = BoolCombination.Both;
        public DemoAccess Access { get; }

        public DemoFilter(DemoAccess demoAccess)
        {
            Access = demoAccess;
        }

        private DemoFilter() : this(DemoAccess.None)
        { }

        public static DemoFilter Active(DemoAccess demoAccess) => new DemoFilter(demoAccess)
            { IsActive = BoolCombination.TrueOnly };
    }
}
