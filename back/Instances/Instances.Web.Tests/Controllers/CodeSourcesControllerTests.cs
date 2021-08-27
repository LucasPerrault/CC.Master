using CloudControl.Web.Tests.Mocks;
using FluentAssertions;
using Instances.Application.CodeSources;
using Instances.Domain.CodeSources;
using Moq;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Tools;
using Xunit;

namespace Instances.Web.Tests.Controllers
{
    public class CodeSourcesControllerTests
    {
        #region GetBuildUrlAsync
        [Fact]
        public async Task GetBuildUrlAsync_Ok()
        {
            var responseUrl = "http://test.local.cc";
            var codeSourcesRepositoryMock = new Mock<ICodeSourcesRepository>(MockBehavior.Strict);
            codeSourcesRepositoryMock
                .Setup(c => c.GetBuildUrlAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(responseUrl);

            var webApplicationFactory = new MockedWebApplicationFactory();
            webApplicationFactory.AddSingleton(codeSourcesRepositoryMock);

            var httpClient = webApplicationFactory.CreateAuthenticatedClient();

            var response = await httpClient.GetAsync(
                "/api/code-sources/services/build-url?CodeSourceCode=Figgo&branchName=MyBranch&BuildNumber=1234"
            );

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();

            codeSourcesRepositoryMock
                .Setup(c => c.GetBuildUrlAsync("Figgo", "MyBranch", "1234"))
                .ReturnsAsync(responseUrl);
            content.Should().Be($"{{\"url\":\"{responseUrl}\"}}");
        }

        [Fact]
        public async Task GetBuildUrlAsync_NotFound()
        {
            var codeSourcesRepositoryMock = new Mock<ICodeSourcesRepository>(MockBehavior.Strict);
            codeSourcesRepositoryMock
                .Setup(c => c.GetBuildUrlAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync("");

            var webApplicationFactory = new MockedWebApplicationFactory();
            webApplicationFactory.AddSingleton(codeSourcesRepositoryMock.Object);

            var httpClient = webApplicationFactory.CreateAuthenticatedClient();

            var response = await httpClient.GetAsync(
                "/api/code-sources/services/build-url?CodeSourceCode=Figgo&branchName=MyBranch&BuildNumber=1234"
            );

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Theory]
        [InlineData("branch", "1234", HttpStatusCode.OK)]
        [InlineData("branch", "", HttpStatusCode.OK)]
        [InlineData("", "1234", HttpStatusCode.BadRequest)]
        [InlineData("My/branch", "1234", HttpStatusCode.BadRequest)]
        [InlineData("Mybranch", "bad/branch", HttpStatusCode.BadRequest)]
        public async Task GetBuildUrlAsync_InputValidation(string branchName, string buildNumber, HttpStatusCode expectedStatus)
        {
            var responseUrl = "http://test.local.cc";
            var codeSourcesRepositoryMock = new Mock<ICodeSourcesRepository>(MockBehavior.Strict);
            codeSourcesRepositoryMock
                .Setup(c => c.GetBuildUrlAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(responseUrl);

            var webApplicationFactory = new MockedWebApplicationFactory();
            webApplicationFactory.AddSingleton(codeSourcesRepositoryMock);

            var httpClient = webApplicationFactory.CreateAuthenticatedClient();

            var response = await httpClient.GetAsync(
                $"/api/code-sources/services/build-url?CodeSourceCode=Figgo&branchName={branchName}&BuildNumber={buildNumber}"
            );

            response.StatusCode.Should().Be(expectedStatus);
        }
        #endregion

        #region GetArtifactsListAsync
        [Fact]
        public async Task GetArtifactsListAsync_Ok()
        {
            var codeSourcesRepositoryMock = new Mock<ICodeSourcesRepository>(MockBehavior.Strict);
            codeSourcesRepositoryMock
                .Setup(c => c.GetArtifactsAsync(It.IsAny<int>()))
                .ReturnsAsync(new List<CodeSourceArtifacts>
                {
                    new CodeSourceArtifacts()
                });

            var webApplicationFactory = new MockedWebApplicationFactory();
            webApplicationFactory.AddSingleton(codeSourcesRepositoryMock);

            var httpClient = webApplicationFactory.CreateAuthenticatedClient();

            var response = await httpClient.GetAsync("/api/code-sources/23/artifacts");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await Serializer.DeserializeAsync<List<object>>(await response.Content.ReadAsStreamAsync());
            content.Should().HaveCount(1);

            codeSourcesRepositoryMock.Verify(c => c.GetArtifactsAsync(23));
        }
        #endregion
    }
}
