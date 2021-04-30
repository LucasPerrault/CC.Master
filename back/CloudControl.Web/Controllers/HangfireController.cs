using Authentication.Web.Attributes;
using Instances.Application.Demos.Deletion;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CloudControl.Web.Controllers
{
    [ApiController, Route("/api/hangfire")]
    public class HangfireController
    {
        private readonly InactiveDemosCleaner _cleaner;

        public HangfireController(InactiveDemosCleaner cleaner)
        {
            _cleaner = cleaner;
        }

        [HangfireAuthorize]
        [HttpPost("cleanup-demos")]
        public async Task CleanupDemosAsync()
        {
            await _cleaner.CleanAsync();
        }
    }
}
