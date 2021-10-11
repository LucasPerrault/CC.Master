using System.Collections.Generic;
using Tools;

namespace Instances.Domain.Demos.Filtering
{
    public class DemoFilter
    {
        public CompareString Subdomain { get; set; } = CompareString.Bypass;
        public string Search { get; set; }
        public CompareBoolean IsActive { get; set; } = CompareBoolean.Bypass;
        public CompareBoolean IsTemplate { get; set; } = CompareBoolean.Bypass;
        public CompareBoolean IsProtected { get; set; } = CompareBoolean.Bypass;
        public int? DistributorId { get; set; }
        public int? AuthorId { get; set; }
        public HashSet<string> Clusters { get; set; } = new HashSet<string>();

        public static DemoFilter Active() => new DemoFilter { IsActive = CompareBoolean.TrueOnly };
    }
}
