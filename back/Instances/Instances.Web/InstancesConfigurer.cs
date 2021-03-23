using Instances.Application.Demos;
using Instances.Domain.Demos;
using Instances.Domain.Instances;
using Instances.Infra.DbDuplication;
using Instances.Infra.Demos;
using Instances.Infra.Instances;
using Instances.Infra.Instances.Services;
using Instances.Infra.Storage.Stores;
using Lucca.Core.Api.Abstractions;
using Lucca.Core.Api.Web;
using Microsoft.Extensions.DependencyInjection;

namespace Instances.Web
{
    public static class InstancesConfigurer
    {
        public class InstancesConfiguration
        {
            public IdentityAuthenticationConfig Identity { get; set; }
        }

        public static void ConfigureServices(IServiceCollection services, InstancesConfiguration configuration)
        {
            services.AddSingleton(configuration.Identity);
            services.AddSingleton<IDatabaseDuplicator, DatabaseDuplicator>();
            services.AddSingleton<SqlScriptPicker>();

            services.AddScoped<IDemosStore, DemosStore>();
            services.AddScoped<IDemoRightsFilter, DemoRightsFilter>();
            services.AddScoped<DemosRepository>();
            services.AddScoped<ISubdomainValidator, SubdomainValidator>();

            services.AddScoped<IDemoUsersPasswordResetService, DemoUsersPasswordResetService>();

            services.AddScoped<IUsersPasswordResetService, UsersPasswordResetService>();
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
