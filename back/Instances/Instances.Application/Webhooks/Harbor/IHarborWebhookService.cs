using Instances.Application.Webhooks.Harbor.Models;
using System.Threading.Tasks;

namespace Instances.Application.Webhooks.Harbor
{
    public interface IHarborWebhookService
    {
        Task HandleWebhookAsync(HarborWebhookPayload payload);
    }
}
