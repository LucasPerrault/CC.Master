using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Users.Domain;

namespace Users.Web
{

    [ApiController, Route("/api/users")]
    public class UsersController
    {
        private readonly IUsersSyncService _syncService;

        public UsersController(IUsersSyncService syncService)
        {
            _syncService = syncService;
        }

        [HttpPost("sync")]
        public async Task Sync()
        {
            await _syncService.SyncAsync();
        }
    }
}
