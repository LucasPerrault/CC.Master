using Environments.Infra.Storage.Stores;
using FluentAssertions;
using Lucca.Core.Shared.Domain.Exceptions;
using Moq;
using Moq.Protected;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Environment = Environments.Domain.Environment;

namespace Environments.Infra.Tests
{
    public class EnvironmentsRemoteStoreTests
    {
        private readonly Mock<HttpMessageHandler> _mockMessageHandler;
        private readonly EnvironmentsRemoteStore _environmentsRemoteStore;

        public EnvironmentsRemoteStoreTests()
        {
            _mockMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            _environmentsRemoteStore = new EnvironmentsRemoteStore(
                new HttpClient(_mockMessageHandler.Object)
                {
                    BaseAddress = new Uri("http://cc-legacy")
                }
            );
        }

        #region UpdateSubDomainAsync
        [Fact]
        public async Task UpdateSubDomainAsync_Ok()
        {
            var environment = new Environment
            {
                Id = 42,
                Subdomain = "oldSubDomain"
            };
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("")
            };
            _mockMessageHandler
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(response);

            await _environmentsRemoteStore.UpdateSubDomainAsync(environment, "newSubDomain");
        }

        [Fact]
        public async Task UpdateSubDomainAsync_BadRequest()
        {
            var environment = new Environment
            {
                Id = 42,
                Subdomain = "oldSubDomain"
            };
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = new StringContent("")
            };
            _mockMessageHandler
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(response);

            Func<Task> act = () => _environmentsRemoteStore.UpdateSubDomainAsync(environment, "newSubDomain");

            (await act
                .Should().ThrowAsync<DomainException>())
                .And.Status.Should().Be(DomainExceptionCode.BadRequest);
        }

        [Fact]
        public async Task UpdateSubDomainAsync_Forbidden()
        {
            var environment = new Environment
            {
                Id = 42,
                Subdomain = "oldSubDomain"
            };
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.Forbidden,
                Content = new StringContent("")
            };
            _mockMessageHandler
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(response);

            Func<Task> act = () => _environmentsRemoteStore.UpdateSubDomainAsync(environment, "newSubDomain");

            (await act
                .Should().ThrowAsync<DomainException>())
                .And.Status.Should().Be(DomainExceptionCode.InternalServerError);
        }
        #endregion
    }
}
