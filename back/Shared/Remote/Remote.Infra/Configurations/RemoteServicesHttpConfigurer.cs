using System;
using System.Net.Http;

namespace Remote.Infra.Configurations
{
    internal static class RemoteServicesHttpConfigurer
    {

        private static void ConfigureHttpClient<T>(T serviceConfiguration, Uri endpoint, HttpClient client)
            where T : RemoteServiceConfiguration<HostHttpClientConfiguration>
        {
            var authParams = new HostHttpClientConfiguration { Endpoint = endpoint };
            serviceConfiguration.Configure(client, authParams);
        }
    }
}
