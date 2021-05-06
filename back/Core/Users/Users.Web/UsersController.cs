using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tools;
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
        public Task<List<SimpleUser>> GetUsersAsync([FromQuery]UsersQuery query)
        {
            var filter = ToFilter(query);
            return _repository.GetAsync(filter);
        }

        private UsersFilter ToFilter(UsersQuery query)
        {
            return new UsersFilter
            {
                IsActive = BoolCombination.TrueOnly,
                Search = query.Search
            };
        }
    }

    public class UsersQuery
    {
        public string Search { get; set; } = null;
    }
}
