using CloudControl.Web;
using CloudControl.Web.Tests.Mocks;
using FluentAssertions;
using Instances.Application.CodeSources;
using Instances.Domain.CodeSources;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
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
            webApplicationFactory.AddServiceMock(codeSourcesRepositoryMock);

            var httpClient = webApplicationFactory.CreateAuthenticatedClient();

            var response = await httpClient.PostAsync(
                "/api/code-sources/services/build-url",

                new StringContent($@"{{
                    ""CodeSourceCode"": ""Figgo"",
                    ""BrancheName"": ""MyBranch"",
                    ""BuildNumber"": ""1234""
                }}
                ", Encoding.UTF8, "application/json"));

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
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
            webApplicationFactory.AddServiceMock(codeSourcesRepositoryMock);

            var httpClient = webApplicationFactory.CreateAuthenticatedClient();

            var response = await httpClient.PostAsync(
                "/api/code-sources/services/build-url",

                new StringContent($@"{{
                    ""CodeSourceCode"": ""Figgo"",
                    ""BrancheName"": ""MyBranch"",
                    ""BuildNumber"": ""1234""
                }}
                ", Encoding.UTF8, "application/json"));

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
            webApplicationFactory.AddServiceMock(codeSourcesRepositoryMock);

            var httpClient = webApplicationFactory.CreateAuthenticatedClient();

            var response = await httpClient.PostAsync(
                "/api/code-sources/services/build-url",

                new StringContent($@"{{
                    ""CodeSourceCode"": ""Figgo"",
                    ""BrancheName"": ""{branchName}"",
                    ""BuildNumber"": ""{buildNumber}""
                }}
                ", Encoding.UTF8, "application/json"));

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
            webApplicationFactory.AddServiceMock(codeSourcesRepositoryMock);

            var httpClient = webApplicationFactory.CreateAuthenticatedClient();

            var response = await httpClient.GetAsync("/api/code-sources/23/artifacts");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await JsonSerializer.DeserializeAsync<List<object>>(await response.Content.ReadAsStreamAsync());
            content.Should().HaveCount(1);

            codeSourcesRepositoryMock.Verify(c => c.GetArtifactsAsync(23));
        }
        #endregion
    }
}
