using Environments.Domain;
using Environments.Infra.Storage.Stores.Dtos;
using Lucca.Core.Shared.Domain.Exceptions;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Environments.Infra.Storage.Stores
{
    public interface IEnvironmentsRemoteStore
    {
        Task UpdateSubDomainAsync(Environment environement, string newName);
    }
    
    public class EnvironmentsRemoteStore : IEnvironmentsRemoteStore
    {
        private readonly HttpClient _httpClient;

        public EnvironmentsRemoteStore(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task UpdateSubDomainAsync(Environment environement, string newName)
        {
            var body = JsonSerializer.Serialize(new UpdateEnvironmentDto(newName));
            var content = new StringContent(body, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{environement.Id}/updateSubdomain", content);

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                throw new DomainException(DomainExceptionCode.BadRequest, responseBody);
            }
            if (!response.IsSuccessStatusCode)
            {
                throw new DomainException(DomainExceptionCode.InternalServerError, "Failed to contact cc legacy");
            }
        }
    }
}
