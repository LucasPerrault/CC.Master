using Lucca.Core.Shared.Domain.Exceptions;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Tools;

namespace Instances.Application.Webhooks.Github
{
    public abstract class GithubWebhookBaseService<T> : IGithubWebhookService where T : GithubEventPayload
    {
        public async Task HandleEventAsync(Stream jsonStream)
        {
            var payload = await Serializer.DeserializeAsync<T>(jsonStream);
            if (payload == null)
            {
                throw new BadRequestException("Invalid payload");
            }
            await HandleEventAsync(payload);
        }

        abstract protected Task HandleEventAsync(T githubEventPayload);
    }

}
