using Distributors.Domain.Models;
using Instances.Domain.Instances.Models;
using System;

namespace Instances.Domain.Instances
{
    public enum InstanceDuplicationProgress
    {
        Pending = 1,
        Running = 2,
        FinishedWithSuccess = 3,
        FinishedWithFailure = 4,
        Canceled = 5
    }

    public class InstanceDuplication
    {
        public Guid Id { get; set; }

        public string SourceSubdomain { get; set; }
        public string SourceCluster { get; set; }
        public InstanceType SourceType { get; set; }

        public string TargetSubdomain { get; set; }
        public string TargetCluster { get; set; }
        public InstanceType TargetType { get; set; }

        public DateTime StartedAt { get; internal set; }
        public DateTime? EndedAt { get; set; }

        public InstanceDuplicationProgress Progress { get; set; }
        public int DistributorId { get; set; }

        public Distributor Distributor { get; set; }

        public InstanceDuplication()
        {
            StartedAt = DateTime.Now;
        }
    }
}
