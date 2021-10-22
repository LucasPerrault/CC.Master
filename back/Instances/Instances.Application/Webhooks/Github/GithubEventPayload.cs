using System.Text.Json.Serialization;

namespace Instances.Application.Webhooks.Github
{
    public class GithubEventPayload
    {
        public GithubRepository Repository { get; init; }
    }

    public class GithubRepository
    {
        [JsonPropertyName("html_url")]
        public string HtmlUrl { get; init; }
    }
}
