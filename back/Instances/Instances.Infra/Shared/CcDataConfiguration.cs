using System;

namespace Instances.Infra.Shared
{
    public class CcDataConfiguration
    {
        public Guid InboundToken { get; set; }
        public Guid OutboundToken { get; set; }
        public string Scheme { get; set; }
        public string Domain { get; set; }
        public bool ShouldTargetBeta { get; set; }
    }
}
