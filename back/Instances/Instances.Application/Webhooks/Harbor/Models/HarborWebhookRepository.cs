using System.Text.Json.Serialization;

namespace Instances.Application.Webhooks.Harbor.Models
{
    public class HarborWebhookRepository
    {
        [JsonPropertyName("date_created")]
        public long DateCreated { get; init; }
        public string Name { get; init; }
        public string Namespace { get; init; }
        [JsonPropertyName("repo_full_name")]
        public string RepoFullName { get; init; }
        [JsonPropertyName("repo_type")]
        public string RepoType { get; init; }
    }

}
