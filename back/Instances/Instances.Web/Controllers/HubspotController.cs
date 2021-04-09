using Instances.Application.Demos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Instances.Web.Controllers
{

    [ApiController, Microsoft.AspNetCore.Components.Route("/api/hubspot")]
    public class HubspotController
    {
        private readonly HubspotDemoDuplicator _demoDuplicator;

        public HubspotController(HubspotDemoDuplicator demoDuplicator)
        {
            _demoDuplicator = demoDuplicator;
        }

        public class HubspotDemoDuplicationRequest
        {
            public int VId { get; set; }
        }

        [HttpPost("duplications/{instanceDuplicationId}/notify")]
        public async Task<ActionResult> NotifyDuplicationEndAsync
        (
            [FromRoute]Guid instanceDuplicationId,
            [FromQuery]bool isSuccessful
        )
        {
            await _demoDuplicator.MarkAsEndedAsync(instanceDuplicationId, isSuccessful);

            return new StatusCodeResult(StatusCodes.Status202Accepted);
        }

        [HttpPost("/duplication-request")]
        public async Task<ActionResult> RequestDuplicationAsync
        (
            [FromQuery]int successWorkflowId,
            [FromQuery]int failWorkflowId,
            [FromBody]HubspotDemoDuplicationRequest request
        )
        {
            await _demoDuplicator.DuplicateAsync(new HubspotDemoDuplication
            {
                VId = request.VId,
                SuccessWorkflowId = successWorkflowId,
                FailureWorkflowId = failWorkflowId
            });

            return new StatusCodeResult(StatusCodes.Status202Accepted);
        }
    }
}
