using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Users.Application;
using Users.Domain;
using Users.Domain.Filtering;

namespace Users.Web
{

    [ApiController, Route("/api/users")]
    public class UsersController
    {
        private readonly IUsersSyncService _syncService;
        private readonly UsersRepository _repository;

        public UsersController(IUsersSyncService syncService, UsersRepository repository)
        {
            _syncService = syncService;
            _repository = repository;
        }

        [HttpPost("sync")]
        public async Task Sync()
        {
            await _syncService.SyncAsync();
        }

        [HttpGet]
        public Task<List<SimpleUser>> GetUsersAsync()
        {
            return _repository.GetAsync(UsersFilter.ActiveOnly);
        }
    }
}
