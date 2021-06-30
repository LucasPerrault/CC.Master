using Billing.Products.Domain.Interfaces;
using Lucca.Core.Api.Abstractions.Paging;
using Microsoft.AspNetCore.Mvc;
using Rights.Domain;
using Rights.Web.Attributes;
using System.Linq;
using System.Threading.Tasks;

namespace Billing.Web.Controllers
{

    public class SolutionDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public SolutionBusinessUnitDto BusinessUnit { get; set; }
    }

    public class SolutionBusinessUnitDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    [ApiController, Route("/api/solutions")]
    public class SolutionsController
    {
        private readonly ISolutionsStore _solutionsStore;

        public SolutionsController(ISolutionsStore solutionsStore)
        {
            _solutionsStore = solutionsStore;
        }

        [HttpGet, ForbidIfMissing(Operation.ReadCMRR)]
        public async Task<Page<SolutionDto>> GetAsync()
        {
            var solutions = await _solutionsStore.GetAsync();

            var solutionsDto = solutions.Select
            (
                s => new SolutionDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    BusinessUnit = new SolutionBusinessUnitDto
                    {
                        Id = s.BusinessUnitId,
                        Name = s.BusinessUnit.Name
                    }
                }
            ).ToList();

            return new Page<SolutionDto>
            {
                Count = solutionsDto.Count,
                Items = solutionsDto
            };
        }
    }
}
