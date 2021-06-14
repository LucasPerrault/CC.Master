using FluentAssertions;
using Instances.Domain.CodeSources;
using Instances.Infra.CodeSources;
using Instances.Infra.Github;
using Moq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Instances.Infra.Tests.Github
{
    public class CodeSourceFetcherServiceTests
    {
        private readonly Mock<IGithubService> _githubServiceMock = new Mock<IGithubService>();

        [Fact]
        public async Task ShouldGetAppsFromProductionFiles()
        {

            _githubServiceMock
                .Setup(s => s.GetFileContentAsync("mockedUrl", ".cd/production.json"))
                .ReturnsAsync(() => MockedProductionFile.Empty);
            var fetcher = new CodeSourceFetcherService(_githubServiceMock.Object);
            await fetcher.FetchAsync("mockedUrl");
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

            var fetcher = new CodeSourceFetcherService(_githubServiceMock.Object);

            var apps = await fetcher.FetchAsync("mockedUrl");
            apps.Single().Name.Should().Be("My app");
            apps.Single().Code.Should().Be("appCode");
            apps.Single().JenkinsProjectName.Should().Be("JenkinsProjectName");
            apps.Single().Config.Should().BeEquivalentTo(new CodeSourceConfig
            {
                IsPrivate = false,
                IisServerPath = "my-app"
            });
        }

        private static class MockedProductionFile
        {
            public static string Empty => JsonSerializer.Serialize(new { Apps = new List<object>() });
            public static string FromApps(params object[] objects) => JsonSerializer.Serialize(new { Apps = objects });
        }
    }
}
