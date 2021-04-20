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
        private readonly DemosRepository _demosRepository;
        private readonly DeletionCallbackNotifier _notifier;

        public DemosController(DemosRepository demosRepository, DeletionCallbackNotifier notifier)
        {
            _demosRepository = demosRepository;
            _notifier = notifier;
        }

        [HttpGet]
        [ForbidIfMissing(Operation.Demo)]
        public Task<Page<Demo>> GetAsync([FromQuery]DemoListQuery query)
        {
            return _demosRepository.GetDemosAsync(query);
        }

        [HttpDelete("{id:int}")]
        [ForbidIfMissing(Operation.Demo)]
        public Task<Demo> DeleteAsync([FromRoute]int id)
        {
            return _demosRepository.DeleteAsync(id);
        }

        [HttpDelete("deletion-report/{clusterName}")]
        [ForbidIfMissing(Operation.Demo)]
        public Task DeletionReport([FromRoute]string clusterName, [FromBody]DeletionReport deletionReport)
        {
            return _notifier.NotifyDemoDeletionReportAsync(clusterName, deletionReport);
        }
    }
}
