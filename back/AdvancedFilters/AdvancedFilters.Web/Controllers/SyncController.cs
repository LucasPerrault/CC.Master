using AdvancedFilters.Domain.DataSources;
using AdvancedFilters.Infra.Services.Sync;
using Microsoft.AspNetCore.Mvc;
using Rights.Domain;
using Rights.Web.Attributes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdvancedFilters.Web.Controllers
{
    [ApiController, Route("/api/cafe")]
    public class SyncController
    {
        private readonly HugeSyncService _hugeSync;

        public SyncController(HugeSyncService hugeSync)
        {
            _hugeSync = hugeSync;
        }

        [HttpPost("huge-sync")]
        [ForbidIfMissing(Operation.SyncAllCafe)]
        public Task GetAsync([FromQuery]HugeSyncQuery query)
        {
            return _hugeSync.SyncAsync(query.ToFilter());
        }
    }

    public class HugeSyncQuery
    {
        public HashSet<string> Subdomain { get; set; } = new HashSet<string>();

        public SyncFilter ToFilter()
            => new SyncFilter
            {
                Subdomains = Subdomain
            };
    }
}
