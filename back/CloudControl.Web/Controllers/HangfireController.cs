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

        public HangfireController(InactiveDemosCleaner cleaner, IUsersSyncService usersSyncService)
        {
            _cleaner = cleaner;
            _usersSyncService = usersSyncService;
        }

        [HttpPost("cleanup-demos")]
        public async Task CleanupDemosAsync()
        {
            await _cleaner.CleanAsync();
        }

        [HttpPost("sync-users")]
        public async Task SyncUsers()
        {
            await _usersSyncService.SyncAsync();
        }
    }
}
