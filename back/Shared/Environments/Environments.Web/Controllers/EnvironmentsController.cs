using Environments.Application;
using Environments.Domain;
using Lucca.Core.Api.Abstractions.Paging;
using Lucca.Core.Api.Web.ModelBinding.Sorting;
using Microsoft.AspNetCore.Mvc;
using Rights.Domain;
using Rights.Web.Attributes;
using System.Threading.Tasks;

namespace Environments.Web.Controllers
{
    [ApiController, Route("/api/environments")]
    [ApiSort(nameof(Environment.Subdomain))]
    public class EnvironmentsController
    {
        private readonly EnvironmentsRepository _repository;

        public EnvironmentsController(EnvironmentsRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [ForbidIfMissing(Operation.ReadEnvironments)]
        public Task<Page<Environment>> GetUsersAsync([FromQuery]EnvironmentQuery query)
        {
            return _repository.GetAsync(query.Page, query.ToFilter());
        }

        [HttpGet("distributorAccesses")]
        [ForbidIfMissing(Operation.ReadEnvironments)]
        public async Task<Page<EnvironmentWithAccess>> GetAccessesAsync([FromQuery]EnvironmentQuery query)
        {
            var envsWithAccess = await _repository.GetAccessesAsync(query.ToFilter());

            return new Page<EnvironmentWithAccess>
            {
                Count = envsWithAccess.Count,
                Items = envsWithAccess
            };
        }
    }
}
