using Instances.Infra.CodeSources;
using Instances.Infra.Github;
using Moq;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Instances.Infra.Tests.Github
{
    public class CodeSourceFetcherServiceTests
    {
        private readonly Mock<IGithubService> _githubServiceMock = new Mock<IGithubService>();

        [Fact]
        public async Task Should()
        {

            _githubServiceMock
                .Setup(s => s.GetFileContentAsync("mockedUrl", ".cd/production.json"))
                .ReturnsAsync(() => MockedProductionFile.Empty);
            var fetcher = new CodeSourceFetcherService(_githubServiceMock.Object);
            await fetcher.FetchAsync("mockedUrl");
            _githubServiceMock.Verify(s => s.GetFileContentAsync("mockedUrl", ".cd/production.json"), Times.Once);
        }

        private static class MockedProductionFile
        {
            public static string Empty => JsonSerializer.Serialize(new { Apps = new List<object>() });
            public static string FromApps(params object[] objects) => JsonSerializer.Serialize(new { Apps = objects });
        }
    }
}
