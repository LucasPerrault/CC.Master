using CloudControl.Web.Tests.Mocks;
using FluentAssertions;
using Instances.Application.Webhooks.Github;
using Moq;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

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

            var httpClient = webApplicationFactory.CreateAuthenticatedClient();

            if (!string.IsNullOrEmpty(inputEvent))
            {
                httpClient.DefaultRequestHeaders.Add("X-GitHub-Event", inputEvent);
            }
            var content = new StringContent(body, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync("/api/webhooks/github", content);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            mockGithubWebhookServiceProvider.Verify(g => g.GetGithubWebhookService(expectedEvent));
            mockGithubWebhookService.Verify(g => g.HandleEventAsync(It.IsAny<Stream>()));
        }

        #endregion
    }
}
