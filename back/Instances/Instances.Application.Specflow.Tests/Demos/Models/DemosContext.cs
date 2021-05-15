using Instances.Domain.Demos;
using Instances.Domain.Instances.Models;
using Instances.Infra.Storage;
using System;
using System.Collections.Generic;
using Tools.Specflow;

namespace Instances.Application.Specflow.Tests.Demos.Models
{
    public class DemosContext : IDisposable
    {
        public InstancesDbContext DbContext { get; set; }
        public TestPrincipal TestPrincipal { get; set; }
        public DemoTestResults Results { get; } = new DemoTestResults();

        public void Dispose()
        {
            DbContext.Dispose();
        }
    }

    public class DemoTestResults
    {
        public List<Instance> CreatedInstances { get; } = new List<Instance>();
        public List<Demo> Demos { get; set; } = new List<Demo>();
        public Demo SingleDemo { get; set; }
        public Exception Exception { get; set; }
    }
}
