using Shared.Infra.Remote.Services;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Shared.Infra.Remote.Configurations
{
    public class HostServiceConfigurer<TConfiguration> where TConfiguration : RemoteServiceConfiguration<HostHttpClientConfiguration>
    {
        private readonly TConfiguration _configuration;
        private readonly IServiceCollection _services;

        internal HostServiceConfigurer(TConfiguration configuration, IServiceCollection services)
        {
            _configuration = configuration;
            _services = services;
        }

        public void AddRemoteServiceHttpClient<T>(Uri endpoint)
            where T : HostRemoteService<TConfiguration>
        {
            _services.AddRemoteServiceHttpClient<T, TConfiguration>(_configuration, endpoint);
        }
    }

    public static class ChainableConfigurerExtensions
    {
        public static HostServiceConfigurer<TConfiguration> WithHostConfiguration<TConfiguration>(this IServiceCollection services, TConfiguration configuration)
            where TConfiguration : RemoteServiceConfiguration<HostHttpClientConfiguration>
        {
            return new HostServiceConfigurer<TConfiguration>(configuration, services);
        }
    }
}
