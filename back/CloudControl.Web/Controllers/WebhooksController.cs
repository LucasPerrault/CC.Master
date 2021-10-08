using Instances.Application.Webhooks.Github;
using Instances.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CloudControl.Web.Controllers
{
    [ApiController, Route("/api/webhooks")]
    public class WebhooksController : ControllerBase
    {
        private readonly InstancesWebhookHandler _instancesWebhookHandler;

        public WebhooksController(InstancesWebhookHandler instancesWebhookHandler)
        {
            _instancesWebhookHandler = instancesWebhookHandler;
        }

        [HttpPost("github")]
        public Task HandleGithubAsync() => _instancesWebhookHandler.HandleGithubAsync(Request);
    }
}
