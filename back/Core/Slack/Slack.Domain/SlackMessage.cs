namespace Slack.Domain;

public record SlackMessage(string MarkdownText)
{
    public string FallbackText { get; init; } = MarkdownText;
    public static implicit operator SlackMessage(string markdownText) => new(markdownText);
}
