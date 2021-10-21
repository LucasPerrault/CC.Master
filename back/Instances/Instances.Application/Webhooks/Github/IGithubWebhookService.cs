using System.IO;
using System.Threading.Tasks;

namespace Instances.Application.Webhooks.Github
{
    public interface IGithubWebhookService
    {
        Task HandleEventAsync(Stream jsonStream);
    }

}
