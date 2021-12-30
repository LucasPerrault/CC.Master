using Microsoft.Extensions.Logging;
using Slack.Domain;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Slack.Infra;

internal class InternalSlackClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<InternalSlackClient> _logger;

    public InternalSlackClient(HttpClient httpClient, ILogger<InternalSlackClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public Task<SlackMessageChannelInformation> PostMessageAsync(string channel, string? threadId, SlackMessageType type, SlackMessage message)
        => PostOrUpdateMessageAsync("/api/chat.postMessage", channel, threadId, type, message);

    public Task<SlackMessageChannelInformation> UpdateMessageAsync(string channel, string? threadId, SlackMessageType type, SlackMessage message)
    {
        if (threadId is null)
        {
            _logger.LogWarning("Can't send message to {channel} because {threadId} is null", channel, nameof(threadId));
            return Task.FromResult(new SlackMessageChannelInformation(channel, null));
        }
        return PostOrUpdateMessageAsync("/api/chat.update", channel, threadId, type, message);
    }

    private async Task<SlackMessageChannelInformation> PostOrUpdateMessageAsync(string slackApiPath, string channel, string? threadId, SlackMessageType type, SlackMessage message)
    {
        List<object>? attachments = null;
        var blocks = new List<object> { new
        {
            type = "section",
            text = new
            {
                type = "mrkdwn",
                text = message.MarkdownText
            }
        }};
        if (type != SlackMessageType.STANDARD)
        {
            attachments = new List<object>()
            {
                new
                {
                    color = type switch
                    {
                        SlackMessageType.WARNING => "#FF9966",
                        SlackMessageType.INFORMATION => "#4183C4",
                        SlackMessageType.ERROR => "#A30200",
                        SlackMessageType.OK => "#2EB886",
                        _ => throw new NotSupportedException($"{type} is not a valid {nameof(SlackMessageType)}")
                    },
                    fallback = message.FallbackText,
                    blocks
                }
            };
            blocks = new List<object>();
        }


        var content = new StringContent(JsonSerializer.Serialize(new
        {
            channel,
            thread_ts = threadId,
            ts = threadId,
            attachments,
            blocks
        }, new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
        }), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(slackApiPath, content);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            _logger.LogError("Failed to send message, bad http response {httpResponse} : {error}", response.StatusCode, body);
            return new SlackMessageChannelInformation(channel, null);
        }

        using var document = await JsonDocument.ParseAsync(response.Content.ReadAsStream());
        if (!document.RootElement.TryGetProperty("ts", out var tsElement))
        {
            _logger.LogWarning("Failed to extract ts field after posting message");
            return new SlackMessageChannelInformation(channel, null);
        }
        document.RootElement.TryGetProperty("channel", out var newChannelElement);
        var newChannel = newChannelElement.GetString();
        if (newChannel is null)
        {
            _logger.LogWarning("Failed to extract channel field after posting message");
            return new SlackMessageChannelInformation(channel, threadId);
        }
        return new SlackMessageChannelInformation(newChannel, tsElement.GetString());
    }

}
