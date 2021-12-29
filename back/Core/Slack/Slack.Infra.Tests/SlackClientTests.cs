using FluentAssertions;
using FluentAssertions.Execution;
using MELT;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Slack.Domain;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Slack.Infra.Tests;

public class SlackClientTests
{
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly SlackClient _slackClient;

    public SlackClientTests()
    {
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        var httpClient = new HttpClient(_httpMessageHandlerMock.Object)
        {
            BaseAddress = new Uri("http://test")
        };
        _slackClient = new SlackClient(new InternalSlackClient(
            httpClient,
            TestLoggerFactory.Create().CreateLogger<InternalSlackClient>())
        );
    }

    #region SendMessageAsync
    [Fact]
    public async Task ScenarioSendMessageAsync()
    {
        var message = await PostInitialMessageAsync();
        _httpMessageHandlerMock.Invocations.Clear();

        await EditMessageAsync(message);
        _httpMessageHandlerMock.Invocations.Clear();

        await PostThreadMessageAsync(message);
    }

    private async Task<ISlackMessageLifeManager> PostInitialMessageAsync()
    {
        HttpRequestMessage capturedMessage = null;
        _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent(ResponseJson, Encoding.UTF8, "application/json")
               }).Callback<HttpRequestMessage, CancellationToken>((message, _) => capturedMessage = message);

        var message = await _slackClient.SendMessageAsync("#channel", SlackMessageType.ERROR, new SlackMessage("this is a text"));

        _httpMessageHandlerMock
            .Protected()
            .Verify(
               "SendAsync",
               Times.Once(),
               ItExpr.IsAny<HttpRequestMessage>(),
               ItExpr.IsAny<CancellationToken>()
           );
        using (new AssertionScope().WithDefaultIdentifier("initial call"))
        {
            capturedMessage.Should().NotBeNull();
            capturedMessage.Method.Should().Be(HttpMethod.Post);
            capturedMessage.RequestUri.PathAndQuery.Should().Contain("/api/chat.postMessage");
            var content = await capturedMessage.Content.ReadAsStringAsync();
            content.Should().Contain("this is a text");
        }

        return message;
    }
    private async Task EditMessageAsync(ISlackMessageLifeManager message)
    {
        HttpRequestMessage capturedMessage = null;
        _httpMessageHandlerMock
        .Protected()
        .Setup<Task<HttpResponseMessage>>(
          "SendAsync",
          ItExpr.IsAny<HttpRequestMessage>(),
          ItExpr.IsAny<CancellationToken>())
       .ReturnsAsync(new HttpResponseMessage
       {
           StatusCode = HttpStatusCode.OK,
           Content = new StringContent(ResponseJson, Encoding.UTF8, "application/json")
       }).Callback<HttpRequestMessage, CancellationToken>((message, _) => capturedMessage = message);

        await message.EditMessageAsync(new SlackMessage("new test"));
        _httpMessageHandlerMock
            .Protected()
            .Verify("SendAsync", Times.Once(), ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
        using (new AssertionScope().WithDefaultIdentifier("edit call"))
        {
            capturedMessage.Should().NotBeNull();
            capturedMessage.Method.Should().Be(HttpMethod.Post);
            capturedMessage.RequestUri.PathAndQuery.Should().Contain("/api/chat.update");
            var content = await capturedMessage.Content.ReadAsStringAsync();
            content.Should().Contain("new test");
            content.Should().Contain("1503435956.000247", "it's the timestamp of the target message");
        }
    }

    private async Task PostThreadMessageAsync(ISlackMessageLifeManager message)
    {
        HttpRequestMessage capturedMessage = null;
        _httpMessageHandlerMock
        .Protected()
        .Setup<Task<HttpResponseMessage>>(
          "SendAsync",
          ItExpr.IsAny<HttpRequestMessage>(),
          ItExpr.IsAny<CancellationToken>())
       .ReturnsAsync(new HttpResponseMessage
       {
           StatusCode = HttpStatusCode.OK,
           Content = new StringContent(ResponseJson, Encoding.UTF8, "application/json")
       }).Callback<HttpRequestMessage, CancellationToken>((message, _) => capturedMessage = message);

        await message.SendThreadMessageAsync(new SlackMessage("thread message"));
        _httpMessageHandlerMock
            .Protected()
            .Verify("SendAsync", Times.Once(), ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
        using (new AssertionScope().WithDefaultIdentifier("post thread message"))
        {
            capturedMessage.Should().NotBeNull();
            capturedMessage.Method.Should().Be(HttpMethod.Post);
            capturedMessage.RequestUri.PathAndQuery.Should().Contain("/api/chat.postMessage");
            var content = await capturedMessage.Content.ReadAsStringAsync();
            content.Should().Contain("thread message");
            content.Should().Contain("1503435956.000247", "it's the timestamp of the target message");
        }
    }

    private const string ResponseJson = @"
                    {
                        ""ok"": true,
                        ""channel"": ""C1H9RESGL"",
                        ""ts"": ""1503435956.000247"",
                        ""message"": {
                            ""text"": ""Here's a message for you"",
                            ""username"": ""ecto1"",
                            ""bot_id"": ""B19LU7CSY"",
                            ""attachments"": [
                                {
                                    ""text"": ""This is an attachment"",
                                    ""id"": 1,
                                    ""fallback"": ""This is an attachment's fallback""
                                }
                            ],
                            ""type"": ""message"",
                            ""subtype"": ""bot_message"",
                            ""ts"": ""1503435956.000247""
                        }
                    }";

    #endregion
}
