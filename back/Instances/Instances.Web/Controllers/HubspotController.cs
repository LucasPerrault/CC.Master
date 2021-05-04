using Instances.Application.Demos.Duplication;
using Instances.Web.Controllers.Dtos;
using IpFilter.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Instances.Web.Controllers
{

    [ApiController, Route("/api/hubspot")]
    public class HubspotController
    {
        private readonly HubspotDemoDuplicator _demoDuplicator;
        private readonly IDemoDuplicationCompleter _duplicationCompleter;

        public HubspotController(HubspotDemoDuplicator demoDuplicator, IDemoDuplicationCompleter duplicationCompleter)
        {
            _demoDuplicator = demoDuplicator;
            _duplicationCompleter = duplicationCompleter;
        }

        public class HubspotDemoDuplicationRequest
        {
            public int VId { get; set; }
        }

        public class HubspotDemoDuplicationQuery
        {
            public int SuccessWorkflowId { get; set; }
            public int FailWorkflowId { get; set; }
        }

        [HttpPost("duplications/{instanceDuplicationId}/notify")]
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

        // Hubspot needs anonymous access to this route as it cannot authenticate its calls.
        // Ip ranges used by hubspot are inconsistent with their documentation.
        //
        // We have no choice but open this route to any call,
        // knowing that an attacker would need to know valid vIds of hubspot contacts
        // in order to create demos on this route.
        //
        // They would also need to know valid workflow ids for emails to reach contacts.
        [AllowAnonymous]
        [AllowAllIps]
        [HttpPost("duplication-request")]
        public async Task<ActionResult> RequestDuplicationAsync
        (
            [FromQuery]HubspotDemoDuplicationQuery duplicationQuery,
            [FromBody]HubspotDemoDuplicationRequest request
        )
        {
            await _demoDuplicator.DuplicateMasterForHubspotAsync(new HubspotDemoDuplication
            {
                VId = request.VId,
                SuccessWorkflowId = duplicationQuery.SuccessWorkflowId,
                FailureWorkflowId = duplicationQuery.FailWorkflowId
            });

            return new StatusCodeResult(StatusCodes.Status202Accepted);
        }
    }
}
