using AdvancedFilters.Infra.Services.Sync;
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
        private readonly SyncService _syncService;

        public SyncController(SyncService syncService)
        {
            _syncService = syncService;
        }

        [HttpPost("huge")]
        [ForbidIfMissing(Operation.SyncAllCafe)]
        public Task HugeSyncAsync()
        {
            return _syncService.SyncEverythingAsync();
        }

        [HttpPost("multi-tenant")]
        [ForbidIfMissing(Operation.SyncAllCafe)]
        public Task MultiSyncAsync()
        {
            return _syncService.SyncMultiTenantDataAsync();
        }

        [HttpPost("mono-tenant")]
        [ForbidIfMissing(Operation.SyncAllCafe)]
        public Task MonoSyncAsync([FromQuery]SyncQuery query)
        {
            if (!query.Subdomain.Any())
            {
                throw new BadRequestException("Subdomain query param is mandatory");
            }
            return _syncService.SyncMonoTenantDataAsync(query.Subdomain);
        }
    }

    public class SyncQuery
    {
        public HashSet<string> Subdomain { get; set; } = new HashSet<string>();
    }
}
