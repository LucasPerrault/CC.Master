using Instances.Application.Demos;
using Instances.Application.Demos.Deletion;
using Instances.Application.Demos.Duplication;
using Instances.Application.Demos.Emails;
using Instances.Application.Instances;
using Instances.Domain.Demos;
using Instances.Domain.Demos.Cleanup;
using Instances.Domain.Demos.Filtering;
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
using Resources.Translations;
using System;

namespace Instances.Web
{
    public static class InstancesConfigurer
    {
        public class InstancesStoreConfiguration
        {
            public Uri Host { get; set; }
            public string Endpoint { get; set; }
            public Guid Token { get; set; }
        }

        public class InstancesConfiguration
        {
            public InstancesStoreConfiguration InstancesStore { get; set; }
            public IdentityAuthenticationConfig Identity { get; set; }
            public CcDataConfiguration CcData { get; set; }
            public WsAuthConfiguration WsAuth { get; set; }
            public HubspotConfiguration Hubspot { get; set; }
            public SqlScriptPickerConfiguration SqlScriptPicker { get; set; }
            public ClusterSelectorConfiguration DemoClusterSelection { get; set; }
        }

        public static void ConfigureServices(IServiceCollection services, InstancesConfiguration configuration)
        {
            services.AddSingleton(configuration.Identity);
            services.AddSingleton(configuration.CcData);
            services.AddSingleton(configuration.Hubspot);
            services.AddSingleton(configuration.SqlScriptPicker);
            services.AddSingleton(configuration.DemoClusterSelection);
            services.AddSingleton<DeletionCallbackNotifier>();
            services.AddSingleton<IUsersPasswordHelper, UsersPasswordHelper>();
            services.AddSingleton<IDemoDeletionCalculator, DemoDeletionCalculator>();
            services.AddSingleton<ISqlScriptPicker, SqlScriptPicker>();

            services.AddScoped<InstancesDuplicator>();
            services.AddScoped<DemoDuplicator>();
            services.AddScoped<HubspotDemoDuplicator>();
            services.AddScoped<DemoDuplicationCompleter>();

            services.AddScoped<IDemosStore, DemosStore>();
            services.AddScoped<IInstanceDuplicationsStore, InstanceDuplicationsStore>();
            services.AddScoped<IDemoDuplicationsStore, DemoDuplicationsStore>();
            services.AddScoped<DemoRightsFilter>();
            services.AddScoped<DemosRepository>();
            services.AddScoped<InstanceDuplicationsRepository>();
            services.AddScoped<InstanceDuplicationsRepository>();
            services.AddScoped<ISubdomainGenerator, SubdomainGenerator>();
            services.AddScoped<ISubdomainValidator, SubdomainValidator>();
            services.AddScoped<IClusterSelector, ClusterSelector>();

            services.AddScoped<IDemoEmails, DemoEmails>();

            services.AddScoped<Translations>();

            services.AddScoped<IDemoUsersPasswordResetService, DemoUsersPasswordResetService>();

            services.AddScoped<IUsersPasswordResetService, UsersPasswordResetService>();


            services.AddHttpClient<IHubspotService, HubspotService>( client =>
            {
                client.WithUserAgent(nameof(HubspotService))
                    .WithBaseAddress(configuration.Hubspot.ServerUri);
            });

            services.AddHttpClient<IInstancesStore, InstancesRemoteStore>(client =>
            {
                client.WithUserAgent(nameof(InstancesRemoteStore))
                    .WithBaseAddress(configuration.InstancesStore.Host, configuration.InstancesStore.Endpoint)
                    .WithAuthScheme("CloudControl").AuthenticateAsApplication(configuration.InstancesStore.Token);

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
                        .AuthenticateAsApplication(configuration.CcData.OutboundToken);
                });

            services.AddHttpClient<IInstanceSessionLogsService, InstanceSessionLogsService>(c =>
            {
                c.WithUserAgent(nameof(InstanceSessionLogsService));
            });
        }

        public static LuccaApiBuilder ConfigureLuccaApiForInstances(this LuccaApiBuilder luccaApiBuilder)
        {
            luccaApiBuilder.ConfigureSorting<Demo>()
                .Allow(d => d.DeletionScheduledOn)
                .Allow(d => d.Id)
                .Allow(d => d.CreatedAt)
                .Allow(d => d.Subdomain);
            return luccaApiBuilder;
        }
    }
}
