using Instances.Domain.Instances;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Instances.Infra.Instances.Services
{
    public class InstanceSessionLogsService : IInstanceSessionLogsService
    {
        private readonly HttpClient _httpClient;

        public InstanceSessionLogsService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<DateTime> GetLatestAsync(Uri href)
        {
            var uri = new Uri(href, "/api/v3/sessionlogs/latest");
            var dateTimeAsString = await _httpClient.GetStringAsync(uri);
            dateTimeAsString = dateTimeAsString.Replace("\"", string.Empty);
            return DateTime.Parse(dateTimeAsString);
        }
    }
}
