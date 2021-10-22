using System.Text.Json.Serialization;

namespace Instances.Application.Webhooks.Harbor.Models
{
    public class HarborWebhookPayload
    {
        public HarborWebhookType Type { get; set; }
        [JsonPropertyName("occur_at")]
        public long OccurAt { get; set; }
        public string Operator { get; set; }
        [JsonPropertyName("event_data")]
        public HarborWebhookEventData EventData { get; set; }
    }

}
