using Distributors.Domain.Models;
using System;

namespace Instances.Domain.Instances
{
    public class InstanceDuplication
    {
        public Guid Id { get; set; }
        public string SourceSubdomain { get; set; }
        public string TargetSubdomain { get; set; }
        public string SourceCluster { get; set; }
        public string TargetCluster { get; set; }

        public InstanceDuplicationType Type { get; set; }
        public string DistributorId { get; set; }

        public Distributor Distributor { get; set; }
    }

    public enum InstanceDuplicationType
    {
        Production = 1,
        Training = 2,
        Preview = 3,
        Demos = 4
    }
}
