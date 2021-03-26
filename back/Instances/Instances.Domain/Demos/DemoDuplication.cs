using Distributors.Domain.Models;
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
        public Guid ExternalId { get; set; }
        public string Subdomain { get; set; }
        public string Comment { get; set; }
        public string Password { get; set; }
        public string DistributorId { get; set; }
        public int SourceDemoId { get; set; }
        public int AuthorId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DemoDuplicationProgress Progress { get; set; }
        public Distributor Distributor { get; set; }
        public Demo SourceDemo { get; set; }


        public string SourceDemoSubdomain
        {
            get
            {
                if (SourceDemo == null)
                {
                    throw new ApplicationException($"{nameof(DemoDuplication)}.{nameof(SourceDemo)} was not included");
                }

                return SourceDemo.Subdomain;
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
