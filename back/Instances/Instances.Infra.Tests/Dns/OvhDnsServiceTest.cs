using FluentAssertions;
using Instances.Infra.Dns;
using Instances.Infra.Shared;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Ovh.Api;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Instances.Infra.Tests.Dns
{
    public class OvhDnsServiceTest
    {
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly Mock<ILogger<OvhDnsService>> _loggerMock;
        private readonly Client _ovhApiClient;

        public OvhDnsServiceTest()
        {
            _loggerMock = new Mock<ILogger<OvhDnsService>>();
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            _ovhApiClient = new Client("ovh-eu", "app-key", "app-secret", "consumer-key", null, ',', new HttpClient(_httpMessageHandlerMock.Object));
            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),//req => req.RequestUri.PathAndQuery.Contains("/auth/time")),
                  ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent($"{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}")
               });
            //
        }

        #region AddNewCnameAsync
        [Fact]
        public async Task AddNewCnameAsync_ShouldPostToTheCorrectZoneAndRefreshAtTheEnd()
        {
            var isCreationCalled = false;

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.Is<HttpRequestMessage>(req =>
                  req.Method == HttpMethod.Post &&
                  req.RequestUri.PathAndQuery.Contains("/record")),
                  ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("")
               }).Callback(() => isCreationCalled = true);

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.Is<HttpRequestMessage>(req =>
                  req.Method == HttpMethod.Post &&
                  req.RequestUri.PathAndQuery.Contains("/refresh")),
                  ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("")
               }).Callback(() => Assert.True(isCreationCalled));

            var ovhDnsService = new OvhDnsService(_ovhApiClient, _loggerMock.Object);
            var dnsEntryCreation = new DnsEntryCreation
            {
                Cluster = "demo",
                DnsZone = "my-zone",
                Subdomain = "des-maux",
            };

            Func<Task> act = async () => await ovhDnsService.AddNewCnameAsync(dnsEntryCreation);

            await act.Should().NotThrowAsync();

            _httpMessageHandlerMock
                .Protected()
                .Verify(
                   "SendAsync",
                   Times.Once(),
                   ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Post &&
                        req.RequestUri.PathAndQuery.Contains($"/domain/zone/{dnsEntryCreation.DnsZone}/record")),
                   ItExpr.IsAny<CancellationToken>()
               );

            _httpMessageHandlerMock
                .Protected()
                .Verify(
                   "SendAsync",
                   Times.Once(),
                   ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Post &&
                        req.RequestUri.PathAndQuery.Contains($"/domain/zone/{dnsEntryCreation.DnsZone}/refresh")),
                   ItExpr.IsAny<CancellationToken>()
               );
        }

        #endregion

        #region DeleteCnameAsync
        [Fact]
        public async Task DeleteCnameAsync_ShouldNotCallDeleteIfThereIsNoExistingRecord()
        {
            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.Is<HttpRequestMessage>(req =>
                  req.Method == HttpMethod.Get &&
                  req.RequestUri.PathAndQuery.Contains("/record")),
                  ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("[]")
               });

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Delete),
                  ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("")
               });

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Post),
                  ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("")
               });

            var ovhDnsService = new OvhDnsService(_ovhApiClient, _loggerMock.Object);
            var dnsEntryDeletion = new DnsEntryDeletion
            {
                DnsZone = "my-zone",
                Subdomain = "des-maux",
            };

            Func<Task> act = async () => await ovhDnsService.DeleteCnameAsync(dnsEntryDeletion);

            await act.Should().NotThrowAsync();

            _httpMessageHandlerMock
                .Protected()
                .Verify(
                   "SendAsync",
                   Times.Never(),
                   ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Delete),
                   ItExpr.IsAny<CancellationToken>()
               );

            _httpMessageHandlerMock
                .Protected()
                .Verify(
                   "SendAsync",
                   Times.Never(),
                   ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Post),
                   ItExpr.IsAny<CancellationToken>()
               );
        }

        [Fact]
        public async Task DeleteCnameAsync_ShouldDeleteToTheCorrectZoneAndRefreshAtTheEnd()
        {
            var isGetRecordIdCalled = false;
            var isDeletionCalled = false;

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.Is<HttpRequestMessage>(req =>
                  req.Method == HttpMethod.Get &&
                  req.RequestUri.PathAndQuery.Contains("/record")),
                  ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("[1]")
               }).Callback(() => isGetRecordIdCalled = true);

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.Is<HttpRequestMessage>(req =>
                  req.Method == HttpMethod.Delete &&
                  req.RequestUri.PathAndQuery.Contains("/record")),
                  ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("")
               }).Callback(() => {
                   Assert.True(isGetRecordIdCalled, "La récupération du recordId n'a pas été appelée avant la suppression");
                   isDeletionCalled = true;
                });

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.Is<HttpRequestMessage>(req =>
                  req.Method == HttpMethod.Post &&
                  req.RequestUri.PathAndQuery.Contains("/refresh")),
                  ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("")
               }).Callback(() => Assert.True(isDeletionCalled, "La suppression n'a pas été appelée avant le refresh"));

            var ovhDnsService = new OvhDnsService(_ovhApiClient, _loggerMock.Object);
            var dnsEntryDeletion = new DnsEntryDeletion
            {
                DnsZone = "my-zone",
                Subdomain = "des-maux",
            };

            Func<Task> act = async () => await ovhDnsService.DeleteCnameAsync(dnsEntryDeletion);

            await act.Should().NotThrowAsync();

            _httpMessageHandlerMock
                .Protected()
                .Verify(
                   "SendAsync",
                   Times.Once(),
                   ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Delete &&
                        req.RequestUri.PathAndQuery.Contains($"/domain/zone/{dnsEntryDeletion.DnsZone}/record")),
                   ItExpr.IsAny<CancellationToken>()
               );

            _httpMessageHandlerMock
                .Protected()
                .Verify(
                   "SendAsync",
                   Times.Once(),
                   ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Post &&
                        req.RequestUri.PathAndQuery.Contains($"/domain/zone/{dnsEntryDeletion.DnsZone}/refresh")),
                   ItExpr.IsAny<CancellationToken>()
               );
        }

        #endregion

    }
}
