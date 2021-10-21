using FluentAssertions;
using Instances.Domain.CodeSources;
using Instances.Domain.Github;
using Instances.Infra.CodeSources;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Remote.Infra.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Tools;
using Xunit;

namespace Instances.Infra.Tests.Github
{
    public class CodeSourceFetcherServiceTests
    {
        private readonly Mock<IGithubService> _githubServiceMock;
        private readonly Mock<HttpClientHandler> _httpHandlerMock;
        private readonly CodeSourceFetcherService _codeSourceFetcherService;

        public CodeSourceFetcherServiceTests()
        {
            _githubServiceMock = new Mock<IGithubService>(MockBehavior.Strict);
            _httpHandlerMock = new Mock<HttpClientHandler>(MockBehavior.Strict);
            var loggerMock = new Mock<ILogger<CodeSourceFetcherService>>();
            _codeSourceFetcherService = new CodeSourceFetcherService(
                _githubServiceMock.Object,
                new HttpClient(_httpHandlerMock.Object),
                loggerMock.Object
            );
        }

        [Fact]
        public async Task ShouldGetAppsFromProductionFiles()
        {
            _githubServiceMock
                .Setup(s => s.GetFileContentAsync("mockedUrl", ".cd/production.json"))
                .ReturnsAsync(() => MockedProductionFile.Empty);

            await _codeSourceFetcherService.FetchAsync("mockedUrl");

            _githubServiceMock.Verify(s => s.GetFileContentAsync("mockedUrl", ".cd/production.json"), Times.Once);
        }

        [Fact]
        public async Task ShouldProperlyBuildCodeSourcesFromApps()
        {
            _githubServiceMock
                .Setup(s => s.GetFileContentAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(() => MockedProductionFile.FromApps(new
                {
                    Name = "appCode",
                    FriendlyName = "My app",
                    ProjectType = "no-monolith",
                    JenkinsProjectName = "JenkinsProjectName",
                    Path = "my-app"
                }));
            _httpHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                      "SendAsync",
                      ItExpr.Is<HttpRequestMessage>(m => m.RequestUri.ToString() == "http://jenkins2.lucca.local:8080/api/json/?tree=jobs[name,url,jobs[name,url]]"),
                      ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = JsonExtensions.ToJsonPayload(new
                    {
                        _class = "hudson.model.Hudson",
                        jobs = new object[] {
                            new
                            {
                                _class = "com.cloudbees.hudson.plugins.folder.Folder",
                                name = "AzureFunctions",
                                url = "http://jenkins2.lucca.local:8080/job/AzureFunctions/",
                                jobs = new object[] {
                                    new
                                    {
                                        _class = "org.jenkinsci.plugins.workflow.multibranch.WorkflowMultiBranchProject",
                                        name = "JenkinsProjectName",
                                        url = "http://jenkins2.lucca.local:8080/job/AzureFunctions/job/Growth.AzureFunctions/"
                                    }
                                }
                            }
                        }
                    })
                });


            var apps = await _codeSourceFetcherService.FetchAsync("mockedUrl");

            apps.Single().Name.Should().Be("My app");
            apps.Single().Code.Should().Be("appCode");
            apps.Single().JenkinsProjectUrl.Should().Be("http://jenkins2.lucca.local:8080/job/AzureFunctions/job/Growth.AzureFunctions/");
            apps.Single().JenkinsProjectName.Should().Be("JenkinsProjectName");
            apps.Single().Config.Should().BeEquivalentTo(new CodeSourceConfig
            {
                IsPrivate = false,
                IisServerPath = "my-app"
            });
        }

        private static class MockedProductionFile
        {
            public static string Empty => Serializer.Serialize(new { Apps = new List<object>() });
            public static string FromApps(params object[] objects) => Serializer.Serialize(new { Apps = objects });
        }
    }
}
