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
        private readonly UsersRepository _repository;

        public UsersController(UsersRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public Task<List<SimpleUser>> GetUsersAsync()
        {
            return _repository.GetAsync(UsersFilter.ActiveOnly);
        }
    }
}
