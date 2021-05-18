using Lucca.Core.Api.Abstractions.Paging;
using Microsoft.AspNetCore.Mvc;
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
        public Task<Page<SimpleUser>> GetUsersAsync([FromQuery]UsersQuery query)
        {
            var filter = ToFilter(query);
            return _repository.GetAsync(query.Page, filter);
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
        public IPageToken Page { get; set; } = null;
        public string Search { get; set; } = null;
    }
}
