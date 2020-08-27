using Shared.Infra.Remote.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;

namespace Shared.Infra.Remote.Configurations
{
    internal static class RemoteServicesHttpConfigurer
    {
        public static void AddRemoteServiceHttpClient<T, TConfiguration>(this IServiceCollection service, TConfiguration httpConfiguration, Uri endpoint)
            where T : HostRemoteService<TConfiguration>
            where TConfiguration : RemoteServiceConfiguration<HostHttpClientConfiguration>
        {
            service.AddHttpClient<T>(client => ConfigureHttpClient(httpConfiguration, endpoint, client));
        }

        public static void AddRemoteServiceHttpClient<I, T, TConfiguration>(this IServiceCollection service, TConfiguration httpConfiguration, Uri endpoint)
            where T : HostRemoteService<TConfiguration>, I
            where I : class
            where TConfiguration : RemoteServiceConfiguration<HostHttpClientConfiguration>
        {
            service.AddHttpClient<I, T>(client => ConfigureHttpClient(httpConfiguration, endpoint, client));
        }

        private static void ConfigureHttpClient<T>(T serviceConfiguration, Uri endpoint, HttpClient client)
            where T : RemoteServiceConfiguration<HostHttpClientConfiguration>
        {
            var authParams = new HostHttpClientConfiguration { Endpoint = endpoint };
            serviceConfiguration.Configure(client, authParams);
        }
    }
}
