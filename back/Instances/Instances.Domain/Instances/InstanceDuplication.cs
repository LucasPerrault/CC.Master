using Distributors.Domain.Models;
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
        public string TargetSubdomain { get; set; }
        public string SourceCluster { get; set; }
        public string TargetCluster { get; set; }

        public DateTime StartedAt { get; internal set; }
        public DateTime? EndedAt { get; set; }

        public InstanceDuplicationType Type { get; set; }
        public InstanceDuplicationProgress Progress { get; set; }
        public string DistributorId { get; set; }

        public Distributor Distributor { get; set; }

        public InstanceDuplication()
        {
            StartedAt = DateTime.Now;
        }
    }

    public enum InstanceDuplicationType
    {
        Production = 1,
        Training = 2,
        Preview = 3,
        Demos = 4
    }
}
