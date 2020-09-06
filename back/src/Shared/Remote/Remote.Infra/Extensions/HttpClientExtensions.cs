using System;
using System.Net.Http;

namespace Remote.Infra.Extensions
{
    public static class HttpClientExtensions
    {
        public static void SetSafeBaseAddress(this HttpClient httpClient, Uri endpoint)
        {
            httpClient.SetSafeBaseAddress(endpoint.OriginalString);
        }

        public static void SetSafeBaseAddress(this HttpClient httpClient, string endpointPath)
        {
            var fixedPath = endpointPath.EndsWith("/") ? endpointPath : $"{endpointPath}/";
            httpClient.BaseAddress = new Uri(fixedPath);
        }
    }
}
