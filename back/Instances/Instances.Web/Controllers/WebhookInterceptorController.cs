using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Instances.Web.Controllers
{
    [ApiController, Route("/webhooks/github")]
    public class WebhookInterceptorController : ControllerBase
    {
        private readonly InstancesWebhookHandler _instancesWebhookHandler;

        public WebhookInterceptorController(InstancesWebhookHandler instancesWebhookHandler)
        {
            _instancesWebhookHandler = instancesWebhookHandler;
        }

        [HttpPost]
        public async Task InterceptAsync()
        {
            await _instancesWebhookHandler.HandleGithubAsync(Request);
        }
    }
}
