using Instances.Application.Demos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Instances.Web.Controllers
{

    [ApiController, Microsoft.AspNetCore.Components.Route("/api/hubspotDemoDuplication")]
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

        [HttpPost]
        public async Task<ActionResult> RequestDuplicationAsync
        (
            [FromQuery]int successWorkflowId,
            [FromQuery]int failWorkflowId,
            [FromBody] HubspotDemoDuplicationRequest request
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
