using Instances.Application.Webhooks.Github;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Instances.Web
{
    public class InstancesWebhookHandler
    {
        private const string GithubEventHeader = "x-github-event";

        private readonly IGithubWebhookServiceProvider _githubWebhookServiceProvider;

        public InstancesWebhookHandler(IGithubWebhookServiceProvider githubWebhookServiceProvider)
        {
            _githubWebhookServiceProvider = githubWebhookServiceProvider;
        }

        public async Task HandleGithubAsync(HttpRequest request)
        {
            var githubEvent = GetGithubEvent(request.Headers);
            var githubWebhookService = _githubWebhookServiceProvider.GetGithubWebhookService(githubEvent);
            if (githubWebhookService == null)
            {
                return;
            }
            await githubWebhookService.HandleEventAsync(request.Body);
        }

        private static GithubEvent GetGithubEvent(IHeaderDictionary headers)
        {
            var caseInsensitiveHeaders = new Dictionary<string, StringValues>(headers, StringComparer.InvariantCultureIgnoreCase);
            if (!caseInsensitiveHeaders.TryGetValue(GithubEventHeader, out var eventHeaderValue))
            {
                return GithubEvent.NotSupported;
            }
            var rawGithubEvent = eventHeaderValue.FirstOrDefault();

            if (!Enum.TryParse(rawGithubEvent, ignoreCase: true, out GithubEvent githubEvent))
            {
                githubEvent = GithubEvent.NotSupported;
            }
            return githubEvent;
        }
    }
}
