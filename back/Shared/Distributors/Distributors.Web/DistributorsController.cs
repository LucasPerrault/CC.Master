using System.Collections.Generic;
using Distributors.Application;
using Distributors.Domain.Models;
using Lucca.Core.Api.Abstractions.Paging;
using Lucca.Core.Api.Web.ModelBinding.Sorting;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Distributors.Domain;

namespace Distributors.Web
{

    [ApiController, Route("/api/distributors")]
    [ApiSort(nameof(Distributor.Id))]
    public class DistributorsController
    {
        private readonly DistributorsRepository _repository;

        public DistributorsController(DistributorsRepository repository)
        {
            _repository = repository;
        }


        [HttpGet]
        public async Task<Page<Distributor>> GetAsync([FromQuery] DistributorQuery query)
        {
            var distributors = await _repository.GetAsync(query.ToFilter());
            return new Page<Distributor>
            {
                Count = distributors.Count,
                Items = distributors
            };
        }
    }

    public class DistributorQuery
    {
        public HashSet<string> Search { get; set; } = new HashSet<string>();


        public DistributorFilter ToFilter()
        {
            return new DistributorFilter
            {
                Search = Search,
            };
        }
    }
}
