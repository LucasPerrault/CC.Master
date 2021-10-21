using System.Text.Json.Serialization;

namespace Instances.Application.Webhooks.Harbor.Models
{
    public class HarborWebhookResource
    {
        public string Digest { get; init; }
        public string Tag { get; init; }
        [JsonPropertyName("resource_url")]
        public string ResourceUrl { get; init; }
    }

}
