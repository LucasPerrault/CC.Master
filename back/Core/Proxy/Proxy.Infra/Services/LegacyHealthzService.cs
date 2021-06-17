using Microsoft.Extensions.Diagnostics.HealthChecks;
using Remote.Infra.Services;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Core.Proxy.Infra.Services
{
    internal class LegacyHealthzErrorMessage
    {
        public string Message { get; set; }
    }

    public class LegacyHealthzService
    {
        private const string HealthzSubroute = "healthz";

        private readonly HttpClientHelper<LegacyHealthzErrorMessage> _httpClientHelper;

        public LegacyHealthzService(HttpClient httpClient)
        {
            _httpClientHelper = new HttpClientHelper<LegacyHealthzErrorMessage>(httpClient, "Legacy Healthz", e => e.Message);
        }

        internal async Task<HealthCheckResult> GetLegacyHealthAsync()
        {
            var queryParams = new Dictionary<string, string>
            {
                { "fields", LegacyHealthz.ApiFields }
            };
            try
            {
                var legacyHealthzResponse = await _httpClientHelper.GetGenericObjectResponseAsync<LegacyHealthz>(HealthzSubroute, queryParams);
                return legacyHealthzResponse.GetHealthCheckResult();
            }
            catch
            {
                return HealthCheckResult.Unhealthy();
            }
        }

        private class LegacyHealthz
        {
            public static readonly string ApiFields = $"{nameof(Status)}";

            public string Status { get; set; }

            public HealthCheckResult GetHealthCheckResult()
            {
                return Status switch
                {
                    "Healthy" => HealthCheckResult.Healthy(),
                    _ => HealthCheckResult.Unhealthy()
                };
            }
        }
    }
}
