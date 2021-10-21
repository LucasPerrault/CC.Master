using Instances.Application.Webhooks.Harbor.Models;
using Instances.Web.Webhooks;
using IpFilter.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rights.Domain;
using Rights.Web.Attributes;
using System.Threading.Tasks;

namespace CloudControl.Web.Controllers
{
    [ApiController, Route("/api/webhooks")]
    [AllowAllIps]
    public class WebhooksController : ControllerBase
    {
        private readonly InstancesWebhookHandler _instancesWebhookHandler;

        public WebhooksController(InstancesWebhookHandler instancesWebhookHandler)
        {
            _instancesWebhookHandler = instancesWebhookHandler;
        }

        [HttpPost("github")]
        [AllowAnonymous]
        public Task<IActionResult> HandleGithubAsync() => _instancesWebhookHandler.HandleGithubAsync(Request);

        [HttpPost("harbor")]
        [ForbidIfMissing(Operation.EditGitHubBranchesAndPR)]
        public Task<IActionResult> HandleHarborAsync([FromBody] HarborWebhookPayload payload) => _instancesWebhookHandler.HandleHarborAsync(payload);
    }
}
