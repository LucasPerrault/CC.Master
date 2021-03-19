using Distributors.Domain.Models;
using Instances.Domain.Instances.Models;
using System;

namespace Instances.Domain.Demos
{
    public class Demo
    {
        public int Id { get; set; }
        public string Subdomain { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime DeletionScheduledOn { get; set; }
        public bool IsActive { get; set; }
        public int InstanceID { get; set; }
        public Instance Instance { get; set; }
        public string DistributorID { get; set; }
        public Distributor Distributor { get; set; }
        public string Comment { get; set; }
        public bool IsTemplate { get; set; }
    }
}
