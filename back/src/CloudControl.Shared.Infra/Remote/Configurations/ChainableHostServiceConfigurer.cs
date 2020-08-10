using CloudControl.Shared.Infra.Remote.Services;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace CloudControl.Shared.Infra.Remote.Configurations
{
    public class ChainableHostServiceConfigurer<TConfiguration> where TConfiguration : RemoteServiceConfiguration<HostHttpClientConfiguration>
    {
        private readonly TConfiguration _configuration;
        private readonly IServiceCollection _services;

        internal ChainableHostServiceConfigurer(TConfiguration configuration, IServiceCollection services)
        {
            _configuration = configuration;
            _services = services;
        }

        public ChainableHostServiceConfigurer<TConfiguration> AddRemoteServiceHttpClient<I, T>(Uri endpoint)
            where T : HostRemoteService<TConfiguration>, I
            where I : class
        {
            _services.AddRemoteServiceHttpClient<I, T, TConfiguration>(_configuration, endpoint);
            return this;
        }

        public ChainableHostServiceConfigurer<TConfiguration> AddRemoteServiceHttpClient<T>(Uri endpoint)
            where T : HostRemoteService<TConfiguration>
        {
            _services.AddRemoteServiceHttpClient<T, TConfiguration>(_configuration, endpoint);
            return this;
        }
    }

    public static class ChainableConfigurerExtensions
    {
        public static ChainableHostServiceConfigurer<TConfiguration> WithHostConfiguration<TConfiguration>(this IServiceCollection services, TConfiguration configuration)
            where TConfiguration : RemoteServiceConfiguration<HostHttpClientConfiguration>
        {
            return new ChainableHostServiceConfigurer<TConfiguration>(configuration, services);
        }
    }
}
