namespace Slack.Domain;

public interface ISlackMessageLifeManager
{
    Task<ISlackMessageLifeManager> EditMessageAsync(SlackMessage message) => EditMessageAsync(SlackMessageType.STANDARD, message);
    Task<ISlackMessageLifeManager> EditMessageAsync(SlackMessageType type, SlackMessage message);
    Task SendThreadMessageAsync(SlackMessage message) => SendThreadMessageAsync(SlackMessageType.STANDARD, message);
    Task SendThreadMessageAsync(SlackMessageType type, SlackMessage message);
}
