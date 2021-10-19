using AdvancedFilters.Infra.Services.Sync;
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
        private readonly SyncService _syncService;

        public PurgeController(SyncService syncService)
        {
            _syncService = syncService;
        }

        [HttpPost("all")]
        [ForbidIfMissing(Operation.SyncAllCafe)]
        public Task HugeSyncAsync()
        {
            return _syncService.PurgeEverythingAsync();
        }

        [HttpPost("mono-tenant")]
        [ForbidIfMissing(Operation.SyncAllCafe)]
        public Task HugeSyncAsync([FromQuery]HashSet<string> subdomains)
        {
            if (!subdomains.Any())
            {
                throw new ApplicationException("subdomain query param is mandatory");
            }
            return _syncService.PurgeTenantsAsync(subdomains);
        }
    }
}
