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

        public int DistributorId
        {
            get
            {
                ThrowIfInstanceDuplicationIsNotIncluded();
                return InstanceDuplication.DistributorId;
            }
        }

        public bool HasEnded
        {
            get
            {
                ThrowIfInstanceDuplicationIsNotIncluded();
                return InstanceDuplication.EndedAt.HasValue;
            }
        }

        private void ThrowIfInstanceDuplicationIsNotIncluded()
        {
            if (InstanceDuplication == null)
            {
                throw new ApplicationException($"{nameof(DemoDuplication)}.{nameof(InstanceDuplication)} was not included");
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
