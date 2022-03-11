using Instances.Domain.Demos;
using Instances.Domain.Instances;
using Instances.Domain.Instances.Models;
using System.Collections.Generic;

namespace Instances.Application.Specflow.Tests.Demos.Models
{
    public class DemoTestResults
    {
        public List<Instance> CreatedInstances { get; } = new List<Instance>();
        public List<int> DeletedInstanceIds { get; } = new List<int>();
        public List<Demo> Demos { get; } = new List<Demo>();
        public Demo SingleDemo { get; set; }
        public List<DnsEntry> SubdomainPropagations { get; } = new List<DnsEntry>();
    }
}
