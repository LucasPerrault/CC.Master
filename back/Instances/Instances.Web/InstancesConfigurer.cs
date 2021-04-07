using Instances.Application.Demos;
using Instances.Application.Instances;
using Instances.Domain.Demos;
using Instances.Domain.Instances;
using Instances.Domain.Shared;
using Instances.Infra.DataDuplication;
using Instances.Infra.Demos;
using Instances.Infra.Instances;
using Instances.Infra.Instances.Services;
using Instances.Infra.Shared;
using Instances.Infra.Storage.Stores;
using Instances.Infra.WsAuth;
using Lucca.Core.Api.Abstractions;
using Lucca.Core.Api.Web;
using Microsoft.Extensions.DependencyInjection;
using Remote.Infra.Extensions;

namespace Instances.Web
{
    public static class InstancesConfigurer
    {
        public class InstancesConfiguration
        {
            public IdentityAuthenticationConfig Identity { get; set; }
            public CcDataConfiguration CcData { get; set; }
            public WsAuthConfiguration WsAuth { get; set; }
            public HubspotConfiguration Hubspot { get; set; }
        }

        public static void ConfigureServices(IServiceCollection services, InstancesConfiguration configuration)
        {
            services.AddSingleton(configuration.Identity);
            services.AddSingleton(configuration.CcData);
            services.AddSingleton(configuration.Hubspot);
            services.AddSingleton<IUsersPasswordHelper, UsersPasswordHelper>();
            services.AddSingleton<SqlScriptPicker>();

            services.AddScoped<IInstancesStore, InstancesRemoteStore>();

            services.AddScoped<InstancesDuplicator>();

            services.AddScoped<IDemosStore, DemosStore>();
            services.AddScoped<IInstanceDuplicationsStore, InstanceDuplicationsStore>();
            services.AddScoped<IDemoDuplicationsStore, DemoDuplicationsStore>();
            services.AddScoped<IDemoRightsFilter, DemoRightsFilter>();
            services.AddScoped<DemosRepository>();
            services.AddScoped<ISubdomainValidator, SubdomainValidator>();

            services.AddScoped<IDemoUsersPasswordResetService, DemoUsersPasswordResetService>();

            services.AddScoped<IUsersPasswordResetService, UsersPasswordResetService>();

            services.AddHttpClient<IHubspotService, HubspotService>(client =>
            {
                client.WithUserAgent(nameof(HubspotService))
                    .WithBaseAddress(configuration.Hubspot.ServerUri);
            });

            services.AddHttpClient<WsAuthRemoteService>(client =>
            {
                client.WithUserAgent(nameof(WsAuthRemoteService))
                    .WithBaseAddress(configuration.WsAuth.ServerUri, configuration.WsAuth.EndpointPath)
                    .WithAuthScheme("Lucca").AuthenticateAsWebService(configuration.WsAuth.Token);
            });

            services.AddScoped<IWsAuthSynchronizer, WsAuthSynchronizer>();

            services.AddHttpClient<ICcDataService, CcDataService>(
                c =>
                {
                    c.WithUserAgent(nameof(CcDataService))
                        .WithAuthScheme("CloudControl")
                        .AuthenticateAsWebService(configuration.CcData.OutboundToken);
                });
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
