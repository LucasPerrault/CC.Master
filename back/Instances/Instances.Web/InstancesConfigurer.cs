using Instances.Application.Demos;
using Instances.Domain.Demos;
using Instances.Infra.Storage.Stores;
using Lucca.Core.Api.Abstractions;
using Lucca.Core.Api.Web;
using Microsoft.Extensions.DependencyInjection;

namespace Instances.Web
{
    public static class InstancesConfigurer
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IDemosStore, DemosStore>();
            services.AddScoped<IDemoRightsFilter, DemoRightsFilter>();
            services.AddScoped<DemosRepository>();
        }

        public static LuccaApiBuilder ConfigureLuccaApiForInstances(this LuccaApiBuilder luccaApiBuilder)
        {
            luccaApiBuilder.ConfigureSorting<Demo>()
                .Allow(d => d.DeletionScheduledOn)
                .Allow(d => d.Id)
                .Allow(d => d.CreatedAt);
            return luccaApiBuilder;
        }
    }
}
