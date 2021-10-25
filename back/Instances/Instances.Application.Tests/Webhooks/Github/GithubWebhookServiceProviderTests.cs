using FluentAssertions;
using Instances.Application.Webhooks.Github;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using Xunit;

namespace Instances.Application.Tests.Webhooks.Github
{
    public class GithubWebhookServiceProviderTests
    {
        private readonly GithubWebhookServiceProvider _githubWebhookServiceProvider;
        private readonly ServiceProvider _serviceProvider;

        public GithubWebhookServiceProviderTests()
        {
            _serviceProvider = new ServiceCollection()
                .AddSingleton(new PushWebhookService(null, null, null))
                .AddSingleton(new PullRequestWebhookService(null, null, null, null))
                .BuildServiceProvider();
            _githubWebhookServiceProvider = new GithubWebhookServiceProvider(_serviceProvider);
        }

        #region GetGithubWebhookService
        [Theory]
        [InlineData(GithubEvent.Pull_Request)]
        [InlineData(GithubEvent.Push)]
        public void GetGithubWebhookService_Push(GithubEvent githubEvent)
        {
            var result = _githubWebhookServiceProvider.GetGithubWebhookService(githubEvent);

            result.Should().NotBeNull();
        }

        [Fact]
        public void GetGithubWebhookService_NotSupported()
        {
            var result = _githubWebhookServiceProvider.GetGithubWebhookService(GithubEvent.NotSupported);

            result.Should().BeNull();
        }

        [Fact]
        public void GetGithubWebhookService_Invalid()
        {
            Action act = () => _githubWebhookServiceProvider.GetGithubWebhookService((GithubEvent)455466);

            act.Should().Throw<NotImplementedException>();
        }
        #endregion
    }
}
