using Distributors.Domain;
using Instances.Domain.Demos;
using Instances.Domain.Instances;
using Instances.Infra.Storage;
using Lucca.Core.Rights.Abstractions;
using Moq;
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
        public Exception ExceptionResult { get; set; }
        public DemoContextMocks Mocks { get; }

        public DemosContext()
        {
            Mocks = new DemoContextMocks();
        }

        public void Dispose()
        {
            DbContext.Dispose();
        }
    }

    public class DemoContextMocks
    {
        public Mock<IInstancesStore> InstancesStore { get; }
        public Mock<IDistributorsStore> DistributorsStore { get; }

        public DemoContextMocks()
        {
            InstancesStore = new Mock<IInstancesStore>();
            DistributorsStore = new Mock<IDistributorsStore>();
        }
    }
}
