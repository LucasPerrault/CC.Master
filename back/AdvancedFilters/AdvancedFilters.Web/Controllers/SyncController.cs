using AdvancedFilters.Infra.Services.Sync;
using Microsoft.AspNetCore.Mvc;
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
        public Task GetAsync()
        {
            return _hugeSync.SyncAsync();
        }
    }
}
