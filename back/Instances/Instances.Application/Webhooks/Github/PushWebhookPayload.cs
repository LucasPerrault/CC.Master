using System.Text.Json.Serialization;

namespace Instances.Application.Webhooks.Github
{
    public class PushWebhookPayload : GithubEventPayload
    {
        public bool Created { get; init; }
        public bool Deleted { get; init; }
        public string Ref { get; init; }
        public string After { get; init; }
        [JsonPropertyName("head_commit")]
        public PushWebhookCommit HeadCommit { get; init; }
    }

    public class PushWebhookCommit
    {
        public string Id { get; protected set; }

        public string Message { get; protected set; }
    }

}
