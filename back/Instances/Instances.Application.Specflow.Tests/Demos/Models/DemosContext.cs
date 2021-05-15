using Instances.Domain.Demos;
using Instances.Domain.Instances.Models;
using Instances.Infra.Storage;
using Lucca.Core.Rights.Abstractions;
using Rights.Domain;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace Instances.Application.Specflow.Tests.Demos.Models
{
    public class DemosContext : IDisposable
    {
        public InstancesDbContext DbContext { get; set; }
        public ClaimsPrincipal Principal { get; set; }
        public Dictionary<Operation,Scope> OperationsWithScope { get; set; }
        public List<Demo> DemosListResult { get; set; }
        public Demo SingleDemoResult { get; set; }
        public Exception ExceptionResult { get; set; }
        public DemoTestResults Results { get; } = new DemoTestResults();

        public void Dispose()
        {
            DbContext.Dispose();
        }
    }

    public class DemoTestResults
    {
        public List<Instance> CreatedInstances { get; } = new List<Instance>();
    }
}
