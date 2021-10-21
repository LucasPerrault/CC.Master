using Instances.Web.Webhooks;
using IpFilter.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CloudControl.Web.Controllers
{
    [ApiController, Route("/api/webhooks")]
    [AllowAnonymous, AllowAllIps]
    public class WebhooksController : ControllerBase
    {
        private readonly InstancesWebhookHandler _instancesWebhookHandler;

        public WebhooksController(InstancesWebhookHandler instancesWebhookHandler)
        {
            _instancesWebhookHandler = instancesWebhookHandler;
        }

        [HttpPost("github")]
        public Task<IActionResult> HandleGithubAsync() => _instancesWebhookHandler.HandleGithubAsync(Request);
    }
}
