using AdvancedFilters.Application;
using Instances.Application.Demos.Deletion;
using Microsoft.AspNetCore.Mvc;
using Rights.Domain;
using Rights.Web.Attributes;
using System.Threading.Tasks;
using Users.Domain;

namespace CloudControl.Web.Controllers
{
    [ApiController, Route("/api/hangfire"), ForbidIfMissing(Operation.HangfireNetcoreRequest)]
    public class HangfireController
    {
        private readonly InactiveDemosCleaner _cleaner;
        private readonly IUsersSyncService _usersSyncService;
        private readonly Synchronizer _sync;

        public HangfireController(InactiveDemosCleaner cleaner, IUsersSyncService usersSyncService, Synchronizer sync)
        {
            _cleaner = cleaner;
            _usersSyncService = usersSyncService;
            _sync = sync;
        }

        [HttpPost("cleanup-demos")]
        public async Task CleanupDemosAsync([FromQuery]bool isDryRun)
        {
            await _cleaner.CleanAsync(new DemoCleanupParams { IsDryRun = isDryRun});
        }

        [HttpPost("sync-users")]
        public async Task SyncUsers()
        {
            await _usersSyncService.SyncAsync();
        }

        [HttpPost("sync-cafe")]
        public async Task SyncCafe()
        {
            await _sync.PurgeEverythingAsync();
            await _sync.SyncEverythingAsync();
        }
    }
}
