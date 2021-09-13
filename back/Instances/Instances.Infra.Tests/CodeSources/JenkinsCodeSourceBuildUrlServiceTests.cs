using FluentAssertions;
using Instances.Domain.CodeSources;
using Instances.Infra.CodeSources;
using Lucca.Core.Shared.Domain.Exceptions;
using Moq;
using Moq.Protected;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Instances.Infra.Tests.CodeSources
{
    public class JenkinsCodeSourceBuildUrlServiceTests
    {
        private readonly JenkinsCodeSourceBuildUrlService _jenkinsCodeSourceBuildUrlService;
        private readonly Mock<HttpClientHandler> _httpHandlerMock;

        public JenkinsCodeSourceBuildUrlServiceTests()
        {
            _httpHandlerMock = new Mock<HttpClientHandler>(MockBehavior.Strict);
            _jenkinsCodeSourceBuildUrlService = new JenkinsCodeSourceBuildUrlService(new HttpClient(_httpHandlerMock.Object));
        }

        #region IsValidBuildNumber
        [Theory]
        [InlineData("lastSuccessfulBuild", true)]
        [InlineData("lastBuild", true)]
        [InlineData("next-build", false)]
        [InlineData("12743", true)]
        [InlineData("3", true)]
        [InlineData("3.4", false)]
        [InlineData("", true)]
        public void IsValidBuildNumber(string input, bool isValid)
        {
            _jenkinsCodeSourceBuildUrlService.IsValidBuildNumber(input).Should().Be(isValid);
        }
        #endregion

        #region GenerateBuildUrlAsync
        [Theory]
        [InlineData("http://jenkins.lucca.test", null, "http://jenkins.lucca.test/job/my-branch/197")]
        [InlineData("http://jenkins.lucca.test/", null, "http://jenkins.lucca.test/job/my-branch/197")]
        [InlineData("http://jenkins.lucca.test", "lastSuccessfulBuild", "http://jenkins.lucca.test/job/my-branch/197")]
        [InlineData("http://jenkins.lucca.test", "lastBuild", "http://jenkins.lucca.test/job/my-branch/198")]
        [InlineData("http://jenkins.lucca.test/", "1234", "http://jenkins.lucca.test/job/my-branch/1234")]
        public async Task GenerateBuildUrlAsync(string jenkinsProjectUrl, string buildNumber, string expected)
        {
            var branchName = "my-branch";
            var codeSource = new CodeSource
            {
                JenkinsProjectUrl = jenkinsProjectUrl
            };
            _httpHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                      "SendAsync",
                      ItExpr.Is<HttpRequestMessage>(m => m.RequestUri.ToString().StartsWith("http://jenkins.lucca.test/job/my-branch/api/json")),
                      ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(@"
                        {
                            ""lastBuild"": {
                                ""_class"": ""org.jenkinsci.plugins.workflow.job.WorkflowRun"",
                                ""number"": 198,
                                ""url"": ""http://jenkins.lucca.test/job/my-branch/job/master/198/""
                            },
                            ""lastSuccessfulBuild"": {
                                ""_class"": ""org.jenkinsci.plugins.workflow.job.WorkflowRun"",
                                ""number"": 197,
                                ""url"": ""http://jenkins.lucca.test/job/my-branch/job/master/198/""
                            }
                        }
                    ", Encoding.UTF8, "application/json")
                });

            var generatedUrl = await _jenkinsCodeSourceBuildUrlService.GenerateBuildUrlAsync(codeSource, branchName, buildNumber);

            generatedUrl.Should().Be(expected);
        }

        [Fact]
        public async Task GenerateBuildUrlAsync_InvalidJenkins()
        {
            var branchName = "my-branch";
            var codeSource = new CodeSource
            {
                JenkinsProjectUrl = "http://jenkins.lucca.test"
            };
            _httpHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                      "SendAsync",
                      ItExpr.Is<HttpRequestMessage>(m => m.RequestUri.ToString().StartsWith("http://jenkins.lucca.test/job/my-branch/api/json")),
                      ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Content = new StringContent("", Encoding.UTF8, "text/html")
                });

            Func<Task> act = () => _jenkinsCodeSourceBuildUrlService.GenerateBuildUrlAsync(codeSource, branchName, "badBranch");

            await act.Should().ThrowAsync<BadRequestException>();
        }
        #endregion
    }
}
