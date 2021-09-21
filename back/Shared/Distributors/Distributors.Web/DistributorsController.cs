using Distributors.Application;
using Distributors.Domain.Models;
using Lucca.Core.Api.Abstractions.Paging;
using Lucca.Core.Api.Web.ModelBinding.Sorting;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

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
        public async Task<Page<Distributor>> GetAsync()
        {
            var distributors = await _repository.GetAsync();
            return new Page<Distributor>
            {
                Count = distributors.Count,
                Items = distributors
            };
        }
    }
}
