using Instances.Application.Webhooks.Harbor.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Instances.Web.Webhooks
{
    public class InstancesWebhookHandler
    {
        private readonly GithubWebhookHandler _githubHandler;
        private readonly HarborWebhookHandler _harborWebhookHandler;

        public InstancesWebhookHandler(GithubWebhookHandler githubHandler, HarborWebhookHandler harborWebhookHandler)
        {
            _githubHandler = githubHandler;
            _harborWebhookHandler = harborWebhookHandler;
        }

        public Task<IActionResult> HandleGithubAsync(HttpRequest request) => _githubHandler.HandleGithubAsync(request);
        public Task<IActionResult> HandleHarborAsync(HarborWebhookPayload payload) => _harborWebhookHandler.HandleHarborAsync(payload);
    }
}
