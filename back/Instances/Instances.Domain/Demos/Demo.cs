using Distributors.Domain.Models;
using Instances.Domain.Instances.Models;
using System;
using Users.Domain;

namespace Instances.Domain.Demos
{
    public class Demo
    {
        public const string DemoDomain = "ilucca-demo.net";

        public int Id { get; set; }
        public string Subdomain { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime DeletionScheduledOn { get; set; }
        public bool IsActive { get; set; }
        public int AuthorId { get; set; }
        public SimpleUser Author { get; set; }
        public int InstanceID { get; set; }
        public Instance Instance { get; set; }
        public string Cluster { get; set; }
        public int DistributorId { get; set; }
        public Distributor Distributor { get; set; }
        public string Comment { get; set; }
        public bool IsTemplate { get; set; }

        public Uri Href => new Uri($"https://{Subdomain}.{DemoDomain}");
    }
}
