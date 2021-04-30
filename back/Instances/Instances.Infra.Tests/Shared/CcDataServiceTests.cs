using FluentAssertions;
using FluentAssertions.Json;
using Instances.Domain.Shared;
using Instances.Infra.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Instances.Infra.Tests.Shared
{
    public class CcDataServiceTests
    {
        private readonly CcDataService _ccDataService;

        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private readonly CcDataConfiguration _ccDataConfiguration;
        private readonly Mock<ILogger<CcDataService>> _mockLogger;

        public CcDataServiceTests()
        {
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>(MockBehavior.Strict);
            _mockLogger = new Mock<ILogger<CcDataService>>(MockBehavior.Strict);

            _ccDataConfiguration = new CcDataConfiguration
            {
                InboundToken = Guid.NewGuid(),
                Domain = "lucca.local",
                Scheme = "http",
                ShouldTargetBeta = false,
            };

            _ccDataService = new CcDataService(new HttpClient(_mockHttpMessageHandler.Object), _ccDataConfiguration, _mockHttpContextAccessor.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task StartDuplicateInstanceAsync_OkAsync()
        {
            HttpRequestMessage captureMessage = null;

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.Accepted })
               .Callback((HttpRequestMessage message, CancellationToken c) =>
               {
                   captureMessage = message;
               });

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Scheme = "https";
            httpContext.Request.Host = new HostString("cc.ilucca.local");
            _mockHttpContextAccessor
                .Setup(httpContextAccessor => httpContextAccessor.HttpContext)
                .Returns(httpContext);

            await _ccDataService.StartDuplicateInstanceAsync(new DuplicateInstanceRequestDto
            {
                SourceTenant = new TenantDto { Tenant = "tenant" },
                TargetTenant = "target",
                PostRestoreScripts = new List<UriLinkDto> {
                    new UriLinkDto
                    {
                        Uri = new Uri("http://test")
                    }
                }
            }, "cluster5", "callback/path/return");

            _mockHttpMessageHandler
                .Protected()
                .Verify<Task<HttpResponseMessage>>(
                    "SendAsync",
                    Times.Exactly(1),
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>());

            var callbackAuthHeader = $"Cloudcontrol application={_ccDataConfiguration.InboundToken}";
            captureMessage.Should().NotBeNull();
            captureMessage.Method.Should().Be(HttpMethod.Post);
            captureMessage.RequestUri.Should().Be("http://cc-data.c5.lucca.local/api/v1/duplicate-instance");

            var body = JToken.Parse(await captureMessage.Content.ReadAsStringAsync());
            body.Should().BeEquivalentTo(JToken.Parse($@"{{
                ""CallbackUri"": ""https://cc.ilucca.local/callback/path/return"",
                ""CallbackAuthorizationHeader"": ""{ callbackAuthHeader }"",
                ""SourceTenant"": {{
                    ""Tenant"": ""tenant"",
                    ""CcDataServerUri"": null
                }},
                ""TargetTenant"": ""target"",
                ""PostRestoreScripts"": [
                    {{ ""Uri"": ""http://test"", ""AuthorizationHeader"": null }}
                ]
            }}"));
        }

        [Theory]
        [InlineData("CLUSTER5", "http://cc-data.c5.lucca.local")]
        [InlineData("CLUSTER10", "http://cc-data.c10.lucca.local")]
        [InlineData("DEMO2", "http://cc-data.dm2.lucca.local")]
        [InlineData("DEMO", "http://cc-data.dm1.lucca.local")]
        [InlineData("GREEN3", "http://cc-data.ch.lucca.local")]
        [InlineData("Preview", "http://cc-data.pm.lucca.local")]
        [InlineData("SECURITY", "http://cc-data.se.lucca.local")]
        [InlineData("RECETTE", "http://cc-data.re.lucca.local")]
        public void GetCcDataBaseUri_BetaFalseOk(string input, string expectedOutput)
        {
            var result = _ccDataService.GetCcDataBaseUri(input);

            result.Should().Be(new Uri(expectedOutput));
        }

        [Theory]
        [InlineData("CLUSTER5", "http://cc-data.beta.c5.lucca.local")]
        [InlineData("CLUSTER10", "http://cc-data.beta.c10.lucca.local")]
        [InlineData("DEMO2", "http://cc-data.beta.dm2.lucca.local")]
        [InlineData("DEMO", "http://cc-data.beta.dm1.lucca.local")]
        [InlineData("GREEN3", "http://cc-data.beta.ch.lucca.local")]
        [InlineData("Preview", "http://cc-data.beta.pm.lucca.local")]
        [InlineData("SECURITY", "http://cc-data.beta.se.lucca.local")]
        [InlineData("RECETTE", "http://cc-data.beta.re.lucca.local")]
        public void GetCcDataBaseUri_BetaTrueOk(string input, string expectedOutput)
        {
            _ccDataConfiguration.ShouldTargetBeta = true;
            var result = _ccDataService.GetCcDataBaseUri(input);

            result.Should().Be(new Uri(expectedOutput));
        }
    }
}
