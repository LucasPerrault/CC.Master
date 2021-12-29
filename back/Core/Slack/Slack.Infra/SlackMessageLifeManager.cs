using Slack.Domain;

namespace Slack.Infra;

internal class SlackMessageLifeManager : ISlackMessageLifeManager
{
    private readonly InternalSlackClient _slackClient;
    private readonly string _channel;
    private readonly string? _threadId;

    public SlackMessageLifeManager(InternalSlackClient slackClient, string channel, string? threadId)
    {
        _slackClient = slackClient;
        _channel = channel;
        _threadId = threadId;
    }

    public async Task<ISlackMessageLifeManager> EditMessageAsync(SlackMessageType type, SlackMessage message)
    {
        var newThreadId = await _slackClient.UpdateMessageAsync(_channel, _threadId, type, message);

        return new SlackMessageLifeManager(
            _slackClient,
            _channel,
            newThreadId
        );
    }

    public Task SendThreadMessageAsync(SlackMessageType type, SlackMessage message)
    {
        return _slackClient.PostMessageAsync(_channel, _threadId, type, message);
    }
}
