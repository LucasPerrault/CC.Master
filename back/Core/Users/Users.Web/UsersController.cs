using System.Collections.Generic;
using Lucca.Core.Api.Abstractions.Paging;
using Lucca.Core.Api.Web.ModelBinding.Sorting;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Tools;
using Users.Application;
using Users.Domain;
using Users.Domain.Filtering;

namespace Users.Web
{
    [ApiController, Route("/api/users")]
    [ApiSort("FirstName,LastName")]
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
            return _repository.GetAsync(query.Page, query.ToFilter());
        }
    }

    public class UsersQuery
    {
        public IPageToken Page { get; set; } = null;
        public HashSet<string> Search { get; set; } = null;

        public UsersFilter ToFilter()
        {
            return new UsersFilter
            {
                IsActive = CompareBoolean.TrueOnly,
                Search = Search
            };
        }
    }
}
