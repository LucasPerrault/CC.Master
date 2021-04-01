using FluentAssertions;
using FluentAssertions.Json;
using Instances.Infra.Shared;
using Microsoft.AspNetCore.Http;
using Moq;
using Moq.Protected;
using Newtonsoft.Json.Linq;
using System;
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

        public CcDataServiceTests()
        {
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>(MockBehavior.Strict);

            _ccDataConfiguration = new CcDataConfiguration
            {
                InboundToken = Guid.NewGuid()
            };

            _ccDataService = new CcDataService(new HttpClient(_mockHttpMessageHandler.Object), _ccDataConfiguration, _mockHttpContextAccessor.Object);
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

            await _ccDataService.StartDuplicateInstanceAsync(new Domain.Shared.DuplicateInstanceRequestDto
            {
                SourceTenant = new Domain.Shared.TenantDto { Tenant = "tenant" },
                TargetTenant = "target",
                PostRestoreScripts = new System.Collections.Generic.List<Domain.Shared.UriLinkDto> {
                    new Domain.Shared.UriLinkDto
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
        [InlineData("DEMO", "http://cc-data.dm.lucca.local")]
        [InlineData("GREEN3", "http://cc-data.ch.lucca.local")]
        [InlineData("Preview", "http://cc-data.pm.lucca.local")]
        [InlineData("SECURITY", "http://cc-data.se.lucca.local")]
        public void GetCcDataBaseUri_Ok(string input, string expectedOutput)
        {
            var result = _ccDataService.GetCcDataBaseUri(input);

            result.Should().Be(new Uri(expectedOutput));
        }
    }
}
