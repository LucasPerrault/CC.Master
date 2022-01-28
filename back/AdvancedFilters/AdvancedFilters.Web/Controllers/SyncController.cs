using AdvancedFilters.Application;
using AdvancedFilters.Domain.DataSources;
using AdvancedFilters.Web.Controllers.Locks;
using Lock.Web;
using Lucca.Core.Shared.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Rights.Domain;
using Rights.Web.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdvancedFilters.Web.Controllers
{
    [ApiController, Route("/api/cafe/sync")]
    public class SyncController
    {
        private readonly Synchronizer _sync;

        public SyncController(Synchronizer sync)
        {
            _sync = sync;
        }

        [HttpPost("huge")]
        [OneRequestAtATime(AdvancedFiltersSyncLock.Name, AdvancedFiltersSyncLock.TimeoutInSeconds)]
        [ForbidIfMissing(Operation.SyncAllCafe)]
        public Task HugeSyncAsync()
        {
            return _sync.SyncEverythingAsync();
        }

        [HttpPost("multi-tenant")]
        [OneRequestAtATime(AdvancedFiltersSyncLock.Name, AdvancedFiltersSyncLock.TimeoutInSeconds)]
        [ForbidIfMissing(Operation.SyncAllCafe)]
        public Task MultiSyncAsync()
        {
            return _sync.SyncMultiTenantAsync();
        }

        [HttpPost("mono-tenant")]
        [OneRequestAtATime(AdvancedFiltersSyncLock.Name, AdvancedFiltersSyncLock.TimeoutInSeconds)]
        [ForbidIfMissing(Operation.SyncAllCafe)]
        public Task MonoSyncAsync([FromQuery]SyncQuery query)
        {
            if (!query.Subdomain.Any())
            {
                throw new BadRequestException("Subdomain query param is mandatory");
            }
            return _sync.SyncMonoTenantAsync(query.Subdomain);
        }

        [HttpPost("mono-tenant/random")]
        [OneRequestAtATime(AdvancedFiltersSyncLock.Name, AdvancedFiltersSyncLock.TimeoutInSeconds)]
        [ForbidIfMissing(Operation.SyncAllCafe)]
        public Task RandomMonoSyncAsync([FromQuery]int tenantCount)
        {
            if (tenantCount == 0)
            {
                throw new BadRequestException("tenantCount query param is mandatory");
            }
            return _sync.SyncRandomMonoTenantAsync(tenantCount);
        }
    }

    public class SyncQuery
    {
        public HashSet<string> Subdomain { get; set; } = new HashSet<string>();
    }
}
