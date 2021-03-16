using Instances.Domain.Demos;
using Instances.Infra.Storage;
using Lucca.Core.Api.Abstractions.Paging;
using Lucca.Core.Rights.Abstractions;
using Lucca.Core.Rights.Abstractions.Permissions;
using Rights.Domain;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Instances.Application.Specflow.Tests.Demos.Models
{
    public class DemosContext : IDisposable
    {
        public InstancesDbContext DbContext { get; set; }
        public ClaimsPrincipal Principal { get; set; }
        public Dictionary<Operation,Scope> OperationsWithScope { get; set; }
        public List<Demo> DemosListResult { get; set; }

        public void Dispose()
        {
            DbContext.Dispose();
        }
    }
}
