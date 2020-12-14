using Microsoft.Extensions.DependencyInjection;
using Remote.Infra.Services;
using System;

namespace Remote.Infra.Configurations
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
