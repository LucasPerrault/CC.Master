using System;
using System.Text.Json.Serialization;

namespace Instances.Application.Webhooks.Github
{
    public class PullRequestWebhookPayload : GithubEventPayload
    {
        public string Action { get; init; }
        public int Number { get; init; }

        [JsonPropertyName("pull_request")]
        public PullRequestPayload PullRequest { get; init; }
    }

    public class PullRequestPayload
    {
        [JsonPropertyName("merged_at")]
        public DateTimeOffset? MergedAt { get; init; }
        [JsonPropertyName("closed_at")]
        public DateTimeOffset? ClosedAt { get; init; }
        public string Title { get; init; }
        public GitReferencePayload Head { get; init; }

    }

    public class GitReferencePayload
    {
        public string Ref { get; init; }
    }
}
