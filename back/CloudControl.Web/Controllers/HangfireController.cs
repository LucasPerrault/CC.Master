using Authentication.Web.Attributes;
using Instances.Application.Demos.Deletion;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Users.Domain;

namespace CloudControl.Web.Controllers
{
    [ApiController, Route("/api/hangfire")]
    public class HangfireController
    {
        private readonly InactiveDemosCleaner _cleaner;
        private readonly IUsersSyncService _usersSyncService;

        public HangfireController(InactiveDemosCleaner cleaner, IUsersSyncService usersSyncService)
        {
            _cleaner = cleaner;
            _usersSyncService = usersSyncService;
        }

        [HangfireAuthorize]
        [HttpPost("cleanup-demos")]
        public async Task CleanupDemosAsync()
        {
            await _cleaner.CleanAsync();
        }

        [HangfireAuthorize]
        [HttpPost("sync-users")]
        public async Task SyncUsers()
        {
            await _usersSyncService.SyncAsync();
        }
    }
}
