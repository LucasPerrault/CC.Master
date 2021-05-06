using Instances.Application.Demos.Duplication;
using Instances.Web.Controllers.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Instances.Web.Controllers
{

    [ApiController, Route("/api/hubspot/duplications")]
    public class HubspotController
    {
        private readonly HubspotDemoDuplicator _demoDuplicator;
        private readonly IDemoDuplicationCompleter _duplicationCompleter;

        public HubspotController(HubspotDemoDuplicator demoDuplicator, IDemoDuplicationCompleter duplicationCompleter)
        {
            _demoDuplicator = demoDuplicator;
            _duplicationCompleter = duplicationCompleter;
        }

        [HttpPost("{instanceDuplicationId:guid}/notify")]
        public async Task<ActionResult> NotifyDuplicationEndAsync
        (
            [FromRoute]Guid instanceDuplicationId,
            [FromBody]DuplicationCallbackPayload payload
        )
        {
            await _duplicationCompleter.MarkDuplicationAsCompletedAsync(instanceDuplicationId, payload.Success);
            await _demoDuplicator.MarkAsEndedAsync(instanceDuplicationId, payload.Success);

            return new StatusCodeResult(StatusCodes.Status202Accepted);
        }
    }
}
