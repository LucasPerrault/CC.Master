using CloudControl.Web.Tests.Mocks;
using FluentAssertions;
using Instances.Application.Webhooks.Github;
using Instances.Infra.Github;
using Instances.Web.Webhooks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static Instances.Web.InstancesConfigurer;

namespace Instances.Web.Tests.Controllers
{
    public class InstancesWebhookHandlerTests
    {
        #region HandleGithubAsync
        [Theory]
        [InlineData("push", GithubEvent.Push)]
        [InlineData("pull_Request", GithubEvent.Pull_Request)]
        [InlineData("created", GithubEvent.NotSupported)]
        [InlineData("", GithubEvent.NotSupported)]
        public async Task HandleGithubAsync_Ok(string inputEvent, GithubEvent expectedEvent)
        {
            var body = @"{""id"":12}";
            var mockGithubWebhookServiceProvider = new Mock<IGithubWebhookServiceProvider>(MockBehavior.Strict);
            var mockGithubWebhookService = new Mock<IGithubWebhookService>(MockBehavior.Strict);

            mockGithubWebhookServiceProvider
                .Setup(g => g.GetGithubWebhookService(It.IsAny<GithubEvent>()))
                .Returns(mockGithubWebhookService.Object);
            mockGithubWebhookService
                .Setup(g => g.HandleEventAsync(It.IsAny<Stream>()))
                .Returns(Task.CompletedTask);

            var webApplicationFactory = new MockedWebApplicationFactory();
            webApplicationFactory.Mocks.AddSingleton(mockGithubWebhookServiceProvider.Object);
            webApplicationFactory.Mocks.AddSingleton(new GithubConfiguration
            {
                GithubWebhookSecret = "mysecret"
            });
            webApplicationFactory.Mocks.AddScoped(sp => new InstancesWebhookHandler(sp.GetRequiredService<GithubWebhookHandler>()));
            webApplicationFactory.Mocks.AddScoped(sp => new GithubWebhookHandler(sp.GetRequiredService<IGithubWebhookServiceProvider>(), sp.GetRequiredService<GithubConfiguration>()));

            var httpClient = webApplicationFactory.CreateClient();

            if (!string.IsNullOrEmpty(inputEvent))
            {
                httpClient.DefaultRequestHeaders.Add("X-GitHub-Event", inputEvent);
            }
            httpClient.DefaultRequestHeaders.Add("X-Hub-Signature", "sha1=23a6849b2e0bf119f655363d6236190f2f14e7e5");
            var content = new StringContent(body, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync("/api/webhooks/github", content);

            response.IsSuccessStatusCode.Should().BeTrue();

            mockGithubWebhookServiceProvider.Verify(g => g.GetGithubWebhookService(expectedEvent));
            mockGithubWebhookService.Verify(g => g.HandleEventAsync(It.IsAny<Stream>()));
        }

        #endregion
    }
}
