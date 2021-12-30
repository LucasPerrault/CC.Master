namespace Slack.Domain;

public interface ISlackClient
{
    Task<ISlackMessageLifeManager> SendMessageAsync(string channel, SlackMessage message)
        => SendMessageAsync(channel, SlackMessageType.STANDARD, message);

    Task<ISlackMessageLifeManager> SendMessageAsync(string channel, SlackMessageType type, SlackMessage message);
}
