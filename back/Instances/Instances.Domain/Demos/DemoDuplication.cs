using Instances.Domain.Instances;
using System;

namespace Instances.Domain.Demos
{
    public class DemoDuplication
    {
        public int Id { get; set; }
        public Guid InstanceDuplicationId { get; set; }
        public string Comment { get; set; }
        public string Password { get; set; }
        public int SourceDemoId { get; set; }
        public int AuthorId { get; set; }
        public DateTime CreatedAt { get; set; }
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
        public int SourceId { get; set; }
        public string Subdomain { get; set; }
        public string Comment { get; set; }
        public string Password { get; set; }
        public string DistributorCode { get; set; }
    }
}
