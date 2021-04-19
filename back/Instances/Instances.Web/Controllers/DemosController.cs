using Instances.Application.Demos;
using Instances.Application.Demos.Deletion;
using Instances.Domain.Demos;
using Lucca.Core.Api.Abstractions.Paging;
using Lucca.Core.Api.Web.ModelBinding.Sorting;
using Microsoft.AspNetCore.Mvc;
using Rights.Domain;
using Rights.Web.Attributes;
using System.Threading.Tasks;

namespace Instances.Web.Controllers
{
    [ApiController, Route("/api/demos")]
    [ApiSort("-" + nameof(Demo.Id))]
    public class DemosController
    {
        private DemosRepository _demosRepository;

        public DemosController(DemosRepository demosRepository)
        {
            _demosRepository = demosRepository;
        }

        [HttpGet]
        [ForbidIfMissing(Operation.Demo)]
        public Task<Page<Demo>> GetAsync([FromQuery]DemoListQuery query)
        {
            return _demosRepository.GetDemosAsync(query);
        }

        [HttpDelete("{id}")]
        [ForbidIfMissing(Operation.Demo)]
        public Task<Demo> DeleteAsync([FromRoute]int demoId)
        {
            return _demosRepository.DeleteAsync(demoId);
        }
    }
}
