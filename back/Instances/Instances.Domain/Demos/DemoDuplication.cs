using Instances.Domain.Instances;
using System;

namespace Instances.Domain.Demos
{
    public enum DemoDuplicationProgress
    {
        Pending = 1,
        Running = 2,
        FinishedWithSuccess = 3,
        FinishedWithFailure = 4,
        Canceled = 5
    }

    public class DemoDuplication
    {
        public int Id { get; set; }
        public Guid InstanceDuplicationId { get; set; }
        public string Comment { get; set; }
        public string Password { get; set; }
        public int SourceDemoId { get; set; }
        public int AuthorId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DemoDuplicationProgress Progress { get; set; }
        public InstanceDuplication InstanceDuplication { get; set; }
        public Demo SourceDemo { get; set; }

        public string DistributorId
        {
            get
            {
                if (InstanceDuplication == null)
                {
                    throw new ApplicationException($"{nameof(DemoDuplication)}.{nameof(InstanceDuplication)} was not included");
                }

                return InstanceDuplication.DistributorId;
            }
        }
    }

    public class DemoDuplicationRequest
    {
        public string SourceDemoSubdomain { get; set; }
        public string Subdomain { get; set; }
        public string Comment { get; set; }
        public string Password { get; set; }
        public string DistributorId { get; set; }
    }
}
