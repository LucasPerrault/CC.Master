using Microsoft.Extensions.DependencyInjection;
using System;

namespace Instances.Application.Webhooks.Github
{
    public interface IGithubWebhookServiceProvider
    {
        IGithubWebhookService GetGithubWebhookService(GithubEvent githubEvent);
    }

    public class GithubWebhookServiceProvider : IGithubWebhookServiceProvider
    {
        private IServiceProvider _serviceProvider;

        public GithubWebhookServiceProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IGithubWebhookService GetGithubWebhookService(GithubEvent githubEvent)
            => githubEvent switch
            {
                GithubEvent.NotSupported => null,
                GithubEvent.Push => _serviceProvider.GetRequiredService<PushWebhookService>(),
                GithubEvent.Pull_Request => _serviceProvider.GetRequiredService<PullRequestWebhookService>(),
                _ => throw new NotImplementedException($"Event type non encore géré {githubEvent}")
            };
    }
}
