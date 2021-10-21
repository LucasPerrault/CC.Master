using System.Collections.Generic;

namespace Instances.Application.Webhooks.Harbor.Models
{
    public class HarborWebhookEventData
    {
        public List<HarborWebhookResource> Resources { get; init; }
        public HarborWebhookRepository Repository { get; set; }
    }

}
