using Slack.Domain;

namespace Slack.Infra;

internal record SlackMessageChannelInformation(string Channel, string? ThreadId);

internal class SlackMessageLifeManager : ISlackMessageLifeManager
{
    private readonly InternalSlackClient _slackClient;
    private readonly SlackMessageChannelInformation _channelInformation;

    public SlackMessageLifeManager(InternalSlackClient slackClient, SlackMessageChannelInformation slackMessageChannelInformation)
    {
        _slackClient = slackClient;
        _channelInformation = slackMessageChannelInformation;
    }

    public async Task<ISlackMessageLifeManager> EditMessageAsync(SlackMessageType type, SlackMessage message)
    {
        var info = await _slackClient.UpdateMessageAsync(_channelInformation.Channel, _channelInformation.ThreadId, type, message);

        return new SlackMessageLifeManager(
            _slackClient,
            info
        );
    }

    public Task SendThreadMessageAsync(SlackMessageType type, SlackMessage message)
    {
        return _slackClient.PostMessageAsync(_channelInformation.Channel, _channelInformation.ThreadId, type, message);
    }
}
