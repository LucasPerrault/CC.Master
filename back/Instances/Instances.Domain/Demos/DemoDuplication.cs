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
        public string SourceDemoSubdomain { get; set; }
        public string Subdomain { get; set; }
        public string Comment { get; set; }
        public string Password { get; set; }
        public string DistributorId { get; set; }
        public int AuthorId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DemoDuplicationProgress Progress { get; set; }
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
