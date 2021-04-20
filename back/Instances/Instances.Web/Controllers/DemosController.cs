using Instances.Application.Demos;
using Instances.Application.Demos.Deletion;
using Instances.Application.Demos.Duplication;
using Instances.Domain.Demos;
using Lucca.Core.Api.Abstractions.Paging;
using Lucca.Core.Api.Web.ModelBinding.Sorting;
using Microsoft.AspNetCore.Mvc;
using Rights.Domain;
using Rights.Web.Attributes;
using System;
using System.Threading.Tasks;

namespace Instances.Web.Controllers
{
    [ApiController, Route("/api/demos")]
    [ApiSort("-" + nameof(Demo.Id))]
    public class DemosController
    {
        private readonly DemosRepository _demosRepository;
        private readonly DemoDuplicator _duplicator;
        private readonly DeletionCallbackNotifier _notifier;

        public DemosController
        (
            DemosRepository demosRepository,
            DemoDuplicator duplicator,
            DeletionCallbackNotifier notifier
        )
        {
            _demosRepository = demosRepository;
            _duplicator = duplicator;
            _notifier = notifier;
        }

        [HttpGet]
        [ForbidIfMissing(Operation.Demo)]
        public Task<Page<Demo>> GetAsync([FromQuery]DemoListQuery query)
        {
            return _demosRepository.GetDemosAsync(query);
        }

        [HttpPost("duplicate")]
        [ForbidIfMissing(Operation.Demo)]
        public Task<DemoDuplication> Duplicate(DemoDuplicationRequest request)
        {
            return _duplicator.CreateDuplicationAsync(request, DemoDuplicationRequestSource.Api);
        }

        [HttpDelete("{id:int}")]
        [ForbidIfMissing(Operation.Demo)]
        public Task<Demo> DeleteAsync([FromRoute]int id)
        {
            return _demosRepository.DeleteAsync(id);
        }

        [HttpPost("duplications/{duplicationId:guid}/notify")]
        [ForbidIfMissing(Operation.Demo)]
        public Task DuplicationReport([FromRoute]Guid duplicationId, [FromBody]DuplicationCallbackPayload payload)
        {
            return _duplicator.MarkDuplicationAsCompletedAsync(duplicationId, payload.Success);
        }

        [HttpPost("deletion-report/{clusterName}")]
        [ForbidIfMissing(Operation.Demo)]
        public Task DeletionReport([FromRoute]string clusterName, [FromBody]DeletionReport deletionReport)
        {
            return _notifier.NotifyDemoDeletionReportAsync(clusterName, deletionReport);
        }
    }

    public class DuplicationCallbackPayload
    {
        public bool Success { get; set; }
        public string Error { get; set; }
    }
}
