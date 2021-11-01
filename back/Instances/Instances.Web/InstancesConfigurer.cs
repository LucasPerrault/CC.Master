using Environments.Domain.ExtensionInterface;
using Instances.Application.CodeSources;
using Instances.Application.Demos;
using Instances.Application.Demos.Deletion;
using Instances.Application.Demos.Duplication;
using Instances.Application.Demos.Emails;
using Instances.Application.Instances;
using Instances.Application.Webhooks.Github;
using Instances.Application.Webhooks.Harbor;
using Instances.Domain.CodeSources;
using Instances.Domain.Demos;
using Instances.Domain.Demos.Cleanup;
using Instances.Domain.Demos.Filtering;
using Instances.Domain.Demos.Validation;
using Instances.Domain.Github;
using Instances.Domain.Instances;
using Instances.Domain.Preview;
using Instances.Domain.Renaming;
using Instances.Domain.Shared;
using Instances.Infra.CodeSources;
using Instances.Infra.DataDuplication;
using Instances.Infra.Demos;
using Instances.Infra.Dns;
using Instances.Infra.Github;
using Instances.Infra.Iis;
using Instances.Infra.Instances;
using Instances.Infra.Instances.Services;
using Instances.Infra.Shared;
using Instances.Infra.Storage.Stores;
using Instances.Infra.Windows;
using Instances.Infra.WsAuth;
using Instances.Web.Webhooks;
using Lucca.Core.Api.Abstractions;
using Lucca.Core.Api.Web;
using Microsoft.Extensions.DependencyInjection;
using Octokit;
using Ovh.Api;
using Remote.Infra.Extensions;
using Resources.Translations;
using System;

namespace Instances.Web
{
    public static class InstancesConfigurer
    {
        public class LegacyCloudControlEndpointConfiguration
        {
            public Uri Host { get; set; }
            public string Endpoint { get; set; }
            public Guid Token { get; set; }
        }

        public class InstancesConfiguration
        {
            public LegacyCloudControlEndpointConfiguration InstancesStore { get; set; }
            public LegacyCloudControlEndpointConfiguration CodeSourcesStore { get; set; }
            public LegacyCloudControlEndpointConfiguration PreviewConfigurationsStore { get; set; }
            public IdentityAuthenticationConfig Identity { get; set; }
            public CcDataConfiguration CcData { get; set; }
            public WsAuthConfiguration WsAuth { get; set; }
            public HubspotConfiguration Hubspot { get; set; }
            public ClusterSelectorConfiguration DemoClusterSelection { get; set; }
            public GithubConfiguration Github { get; set; }
            public DnsConfiguration Dns { get; set; }
            public RedirectionIisConfiguration RedirectionIis { get; set; }
        }

        public static void ConfigureServices(IServiceCollection services, InstancesConfiguration configuration)
        {
            services.AddSingleton(configuration.Github);
            services.AddSingleton(configuration.Identity);
            services.AddSingleton(configuration.CcData);
            services.AddSingleton(configuration.Hubspot);
            services.AddSingleton(configuration.DemoClusterSelection);
            services.AddSingleton(configuration.Dns.Internal);
            services.AddSingleton(configuration.Dns.Ovh);
            services.AddSingleton(configuration.Dns.Zones);
            services.AddSingleton<DeletionCallbackNotifier>();
            services.AddSingleton<IUsersPasswordHelper, UsersPasswordHelper>();
            services.AddSingleton<IDemoDeletionCalculator, DemoDeletionCalculator>();
            services.AddSingleton<ISqlScriptPicker, SqlScriptPicker>();

            services.AddSingleton<IDnsService, DnsService>();
            services.AddSingleton<IWmiWrapper, WmiWrapper>();
            services.AddSingleton<IInternalDnsService, WinDnsService>();
            services.AddSingleton(
                sp =>
                {
                    var client = new Client(configuration.Dns.Ovh.Endpoint, configuration.Dns.Ovh.ApplicationKey, configuration.Dns.Ovh.ApplicationSecret, configuration.Dns.Ovh.ConsumerKey);
                    return client;
                });
            services.AddSingleton<IExternalDnsService, OvhDnsService>();

            services.AddSingleton(
                sp =>
                {
                    var client = new GitHubClient(new ProductHeaderValue(configuration.Github.ProductHeaderValue))
                    {
                        Credentials = new Credentials(configuration.Github.Token)
                    };
                    return client;
                });
            services.AddSingleton<IGithubService, GithubService>();

            services.AddScoped<InstancesWebhookHandler>();
            services.AddScoped<GithubWebhookHandler>();
            services.AddScoped<HarborWebhookHandler>();

            services.AddScoped<InactiveDemosCleaner>();
            services.AddScoped<InstancesManipulator>();
            services.AddScoped<DemoDuplicator>();
            services.AddScoped<HubspotDemoDuplicator>();
            services.AddScoped<IDemoDuplicationCompleter, DemoDuplicationCompleter>();

            services.AddScoped<ICodeSourcesRepository, CodeSourcesRepository>();
            services.AddHttpClient<ICodeSourceFetcherService, CodeSourceFetcherService>(c =>
            {
                c.WithUserAgent(nameof(CodeSourceFetcherService));
            });
            services.AddScoped<IGithubBranchesRepository, GithubBranchesRepository>();
            services.AddScoped<IGithubPullRequestsRepository, GithubPullRequestsRepository>();
            services.AddScoped<IPreviewConfigurationsRepository, PreviewConfigurationsRepository>();
            services.AddScoped<IHelmRepository, HelmRepository>();

            services.AddScoped<IDemosStore, DemosStore>();
            services.AddScoped<IInstancesStore, InstancesStore>();
            services.AddScoped<IInstanceDuplicationsStore, InstanceDuplicationsStore>();
            services.AddScoped<IDemoDuplicationsStore, DemoDuplicationsStore>();
            services.AddScoped<DemoRightsFilter>();
            services.AddScoped<DemosRepository>();
            services.AddScoped<InstanceDuplicationsRepository>();
            services.AddScoped<InstanceDuplicationsRepository>();
            services.AddScoped<ISubdomainGenerator, SubdomainGenerator>();
            services.AddScoped<IClusterSelector, ClusterSelector>();
            services.AddScoped<ICodeSourcesStore, CodeSourcesStore>();
            services.AddScoped<IGithubPullRequestsStore, GithubPullRequestsStore>();
            services.AddScoped<IGithubBranchesStore, GithubBranchesStore>();

            services.AddScoped<ISubdomainValidationTranslator, SubdomainValidationTranslator>();
            services.AddScoped<ISubdomainValidator, SubdomainValidator>();

            services.AddScoped<IDemoEmails, DemoEmails>();

            services.AddScoped<Translations>();

            services.AddScoped<IDemoUsersPasswordResetService, DemoUsersPasswordResetService>();

            services.AddScoped<IUsersPasswordResetService, UsersPasswordResetService>();


            services.AddHttpClient<IHubspotService, HubspotService>( client =>
            {
                client.WithUserAgent(nameof(HubspotService))
                    .WithBaseAddress(configuration.Hubspot.ServerUri);
            });

            services.AddHttpClient<IInstancesRemoteStore, InstancesRemoteStore>(client =>
            {
                client.WithUserAgent(nameof(InstancesRemoteStore))
                    .WithBaseAddress(configuration.InstancesStore.Host, configuration.InstancesStore.Endpoint)
                    .WithAuthScheme("CloudControl").AuthenticateAsApplication(configuration.InstancesStore.Token);

            });

            services.AddHttpClient<IPreviewConfigurationsStore, PreviewConfigurationsStore>(client =>
            {
                client.WithUserAgent(nameof(PreviewConfigurationsStore))
                    .WithBaseAddress(configuration.PreviewConfigurationsStore.Host, configuration.PreviewConfigurationsStore.Endpoint)
                    .WithAuthScheme("CloudControl").AuthenticateAsApplication(configuration.PreviewConfigurationsStore.Token);
            });

            services.AddHttpClient<WsAuthRemoteService>(client =>
            {
                client.WithUserAgent(nameof(WsAuthRemoteService))
                    .WithBaseAddress(configuration.WsAuth.ServerApiEndpoint)
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

            services.AddHttpClient<IArtifactsService, ArtifactsService>(c =>
            {
                c.WithUserAgent(nameof(ArtifactsService));
            });

            services.AddHttpClient<ICodeSourceBuildUrlService, JenkinsCodeSourceBuildUrlService>(c =>
            {
                c.WithUserAgent(nameof(ArtifactsService));
            });


            services.AddScoped<IGithubWebhookServiceProvider, GithubWebhookServiceProvider>();
            services.AddScoped<PushWebhookService>();
            services.AddScoped<PullRequestWebhookService>();

            services.AddScoped<IHarborWebhookService, HarborWebhookService>();

            services.AddScoped<IEnvironmentRenamingExtension, InstanceDnsRenaming>();
            services.AddScoped<IEnvironmentRenamingExtension, RedirectionRenaming>();
            services.AddSingleton(configuration.RedirectionIis);
            services.AddSingleton<IRedirectionIisAdministration, RedirectionIisAdministration>();
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
