using Slack.Domain;

namespace Slack.Infra;

internal class SlackClient : ISlackClient
{
    private readonly InternalSlackClient _slackClient;

    public SlackClient(InternalSlackClient slackClient)
    {
        _slackClient = slackClient;
    }

    public async Task<ISlackMessageLifeManager> SendMessageAsync(string channel, SlackMessageType type, SlackMessage message)
    {
        var threadId = await _slackClient.PostMessageAsync(channel, threadId: null, type, message);

        return new SlackMessageLifeManager(
            _slackClient,
            channel,
            threadId
        );
    }
}
