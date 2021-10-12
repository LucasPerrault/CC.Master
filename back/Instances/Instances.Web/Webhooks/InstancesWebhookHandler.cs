using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Instances.Web.Webhooks
{
    public class InstancesWebhookHandler
    {
        private readonly GithubWebhookHandler _githubHandler;

        public InstancesWebhookHandler(GithubWebhookHandler githubHandler)
        {
            _githubHandler = githubHandler;
        }

        public Task<IActionResult> HandleGithubAsync(HttpRequest request) => _githubHandler.HandleGithubAsync(request);
    }
}
