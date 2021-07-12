using FluentAssertions;
using Instances.Domain.CodeSources;
using Instances.Infra.CodeSources;
using Lucca.Core.Shared.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Instances.Infra.Tests.CodeSources
{
    public class ArtifactsServiceTests
    {
        private readonly ArtifactsService _artifactsService;
        private readonly Mock<HttpMessageHandler> _httpHandlerMock;

        public ArtifactsServiceTests()
        {
            _httpHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

            var httpClient = new HttpClient(_httpHandlerMock.Object);
            var loggerMock = new Mock<ILogger<ArtifactsService>>();
            _artifactsService = new ArtifactsService(loggerMock.Object, new JenkinsCodeSourceBuildUrlService(httpClient), httpClient);
        }

        #region GetArtifactsAsync
        [Fact]
        public async Task GetArtifactsAsync_Ok()
        {
            var codeSource = new CodeSource
            {
                JenkinsProjectUrl = "http://jenkins.lucca.test"
            };
            _httpHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                      "SendAsync",
                      ItExpr.IsAny<HttpRequestMessage>(),
                      ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(new
                    {
                        _class = "org.jenkinsci.plugins.workflow.job.WorkflowRun",
                        artifacts = new object[] {
                            new {
                                displayPath= "production.json",
                                fileName= "production.json",
                                relativePath= ".cd/production.json"
                            },
                            new {
                                displayPath= "Figgo.back.zip",
                                fileName= "Figgo.back.zip",
                                relativePath= ".jenkins/zips/Figgo.back.zip"
                            },
                            new {
                                displayPath= "Figgo.front.zip",
                                fileName= "Figgo.front.zip",
                                relativePath= ".jenkins/zips/Figgo.front.zip"
                            },
                            new {
                                displayPath= "anonymization.sql",
                                fileName= "anonymization.sql",
                                relativePath= ".cd/sql/anonymization.sql"
                            },
                            new {
                                displayPath= "clean.sql",
                                fileName= "clean.sql",
                                relativePath= ".cd/sql/clean.sql"
                            },
                            new {
                                displayPath= "Figgo.e2e.zip",
                                fileName= "Figgo.e2e.zip",
                                relativePath= ".jenkins/zips/Figgo.e2e.zip"
                            }
                        }
                    }))
                });

            var result = await _artifactsService.GetArtifactsAsync(codeSource, "myBranch", 1234);

            result.Should().NotBeNullOrEmpty();
            result.Where(a => a.ArtifactType == CodeSourceArtifactType.ProductionJson).Should().HaveCount(1);
            result.Where(a => a.ArtifactType == CodeSourceArtifactType.FrontZip).Should().HaveCount(1);
            result.Where(a => a.ArtifactType == CodeSourceArtifactType.FrontZip).Should().HaveCount(1);
            result.Where(a => a.ArtifactType == CodeSourceArtifactType.AnonymizationScript).Should().HaveCount(1);
            result.Where(a => a.ArtifactType == CodeSourceArtifactType.CleanScript).Should().HaveCount(1);
            result.Where(a => a.ArtifactType == CodeSourceArtifactType.Other).Should().HaveCount(1);
        }

        [Fact]
        public async Task GetArtifactsAsync_NotFound()
        {
            var codeSource = new CodeSource
            {
                JenkinsProjectUrl = "http://jenkins.lucca.test"
            };
            _httpHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                      "SendAsync",
                      ItExpr.IsAny<HttpRequestMessage>(),
                      ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Content = new StringContent("")
                });

            Func<Task> act = () => _artifactsService.GetArtifactsAsync(codeSource, "myBranch", 1234);

            await act.Should().ThrowAsync<BadRequestException>();
        }

        [Fact]
        public async Task GetArtifactsAsync_InternalServerError()
        {
            var codeSource = new CodeSource
            {
                JenkinsProjectUrl = "http://jenkins.lucca.test"
            };
            _httpHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                      "SendAsync",
                      ItExpr.IsAny<HttpRequestMessage>(),
                      ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.InternalServerError
                });

            Func<Task> act = () => _artifactsService.GetArtifactsAsync(codeSource, "myBranch", 1234);

            await act.Should().ThrowAsync<Exception>();
        }
        #endregion
    }
}
