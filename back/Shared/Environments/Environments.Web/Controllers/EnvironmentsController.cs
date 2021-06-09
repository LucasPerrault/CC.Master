using Environments.Application;
using Environments.Domain;
using Lucca.Core.Api.Abstractions.Paging;
using Lucca.Core.Api.Web.ModelBinding.Sorting;
using Microsoft.AspNetCore.Mvc;
using Rights.Domain;
using Rights.Web.Attributes;
using System.Collections.Generic;
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

        public class DistributorWithAccessResponse
        {
            public string Subdomain { get; set; }
            public HashSet<DistributorWithAccess> Accesses { get; set; }
        }

        [HttpGet("distributorAccesses")]
        [ForbidIfMissing(Operation.ReadEnvironments)]
        public async Task<DistributorWithAccessResponse> GetAccessesAsync([FromQuery]string subdomain)
        {
            var accesses = await _repository.GetAccessesAsync(subdomain);
            return new DistributorWithAccessResponse
            {
                Subdomain = subdomain,
                Accesses = accesses
            };
        }
    }
}
