using AdvancedFilters.Application;
using AdvancedFilters.Web.Controllers.Locks;
using Lock.Web;
using Microsoft.AspNetCore.Mvc;
using Rights.Domain;
using Rights.Web.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdvancedFilters.Web.Controllers
{
    [ApiController, Route("/api/cafe/purge")]
    public class PurgeController
    {
        private readonly Synchronizer _sync;

        public PurgeController(Synchronizer sync)
        {
            _sync = sync;
        }

        [HttpPost("all")]
        [OneRequestAtATime(AdvancedFiltersSyncLock.Name, AdvancedFiltersSyncLock.TimeoutInSeconds)]
        [ForbidIfMissing(Operation.SyncAllCafe)]
        public Task HugeSyncAsync()
        {
            return _sync.PurgeEverythingAsync();
        }

        [HttpPost("mono-tenant")]
        [OneRequestAtATime(AdvancedFiltersSyncLock.Name, AdvancedFiltersSyncLock.TimeoutInSeconds)]
        [ForbidIfMissing(Operation.SyncAllCafe)]
        public Task HugeSyncAsync([FromQuery]HashSet<string> subdomains)
        {
            if (!subdomains.Any())
            {
                throw new ApplicationException("subdomain query param is mandatory");
            }
            return _sync.PurgeTenantsAsync(subdomains);
        }
    }
}
