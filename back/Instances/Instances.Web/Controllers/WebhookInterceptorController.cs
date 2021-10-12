using Instances.Web.Webhooks;
using IpFilter.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Instances.Web.Controllers
{
    [ApiController, Route("/webhooks/github")]
    [AllowAnonymous, AllowAllIps]
    public class WebhookInterceptorController : ControllerBase
    {
        private readonly InstancesWebhookHandler _instancesWebhookHandler;

        public WebhookInterceptorController(InstancesWebhookHandler instancesWebhookHandler)
        {
            _instancesWebhookHandler = instancesWebhookHandler;
        }

        [HttpPost]
        public async Task<IActionResult> InterceptAsync()
        {
            return await _instancesWebhookHandler.HandleGithubAsync(Request);
        }
    }
}
