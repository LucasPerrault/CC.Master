using Instances.Application.Demos;
using Instances.Application.Demos.Deletion;
using Instances.Application.Demos.Dtos;
using Instances.Application.Demos.Duplication;
using Instances.Domain.Demos;
using Instances.Web.Controllers.Dtos;
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
        private readonly IDemoDuplicationCompleter _duplicationCompleter;
        private readonly DeletionCallbackNotifier _notifier;

        public DemosController
        (
            DemosRepository demosRepository,
            DemoDuplicator duplicator,
            DeletionCallbackNotifier notifier,
            IDemoDuplicationCompleter duplicationCompleter
        )
        {
            _demosRepository = demosRepository;
            _duplicator = duplicator;
            _notifier = notifier;
            _duplicationCompleter = duplicationCompleter;
        }

        [HttpGet]
        [ForbidIfMissing(Operation.Demo)]
        public Task<Page<Demo>> GetAsync([FromQuery]DemoListQuery query)
        {
            return _demosRepository.GetDemosAsync(query.Page, query.ToDemoFilter());
        }

        [HttpPut("{id:int}")]
        [ForbidIfMissing(Operation.Demo)]
        public Task<Demo> PutAsync([FromRoute]int id, [FromBody]DemoPutPayload payload)
        {
            return _demosRepository.UpdateDemoAsync(id, payload);
        }

        [HttpPost("duplicate")]
        [ForbidIfMissing(Operation.Demo)]
        public Task<DemoDuplication> Duplicate(DemoDuplicationRequest request)
        {
            return _duplicator.CreateDuplicationAsync(request);
        }

        [HttpDelete("{id:int}")]
        [ForbidIfMissing(Operation.Demo)]
        public Task<Demo> DeleteAsync([FromRoute]int id)
        {
            return _demosRepository.DeleteAsync(id);
        }

        [HttpPost("duplications/{duplicationId:guid}/notify")]
        [ForbidIfMissing(Operation.Demo)]
        public Task DuplicationReport
        (
            [FromRoute]Guid duplicationId,
            [FromBody]DuplicationCallbackPayload payload
        )
        {
            return _duplicationCompleter.MarkDuplicationAsCompletedAsync(duplicationId, payload.Success);
        }

        [HttpPost("deletion-report/{clusterName}")]
        [ForbidIfMissing(Operation.Demo)]
        public Task DeletionReport([FromRoute]string clusterName, [FromBody]DeletionReport deletionReport)
        {
            return _notifier.NotifyDemoDeletionReportAsync(clusterName, deletionReport);
        }
    }
}
