using Authentication.Infra.Configurations;
using Billing.Contracts.Infra.Configurations;
using CloudControl.Web.Configuration;
using Core.Proxy.Infra.Configuration;
using Instances.Domain.Demos;
using Instances.Infra.DataDuplication;
using Instances.Infra.Demos;
using Instances.Infra.Instances;
using Instances.Infra.Shared;
using Instances.Infra.WsAuth;
using Instances.Web;
using IpFilter.Infra.Storage;
using IpFilter.Web;
using Lucca.Core.AspNetCore.Healthz;
using Lucca.Core.AspNetCore.Middlewares;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Rights.Infra.Configuration;
using Salesforce.Infra.Configurations;
using System;
using System.Collections.Generic;
using TeamNotification.Web;
using Users.Infra.Storage;

namespace CloudControl.Web.Tests.Mocks
{
    public class TestServicesConfiguration<TAuthenticationHandler> : ServicesConfiguration where TAuthenticationHandler : AuthenticationHandler<TestAuthenticationOptions>
    {
        private readonly AuthenticationConfiguration _mockedAuthConfiguration;

        public TestServicesConfiguration(IConfiguration configuration, IWebHostEnvironment env)
            : base(configuration, env)
        {
            _mockedAuthConfiguration = new AuthenticationConfiguration
            {
                ServerUri = new Uri("https://mocked-partenaires.local"),
                LogoutEndpointPath = "/logout",
                RedirectEndpointPath = "/login",
                UsersEndpointPath = "/users/endpoint/mocked",
                AllUsersEndpointPath = "/all/users/endpoint/path/mocked",
                ApiKeysEndpointPath = "/api-key/endpoint/mock",
                ApiKeysFetcherToken = Guid.Empty
            };
        }

        public override AppConfiguration ConfigureConfiguration(IServiceCollection services)
        {
            services.AddSingleton(new LegacyCloudControlConfiguration { Host = "un.truc.bidon" });
            return null;
        }

        public override void ConfigureLogs(IServiceCollection services)
        {
            services.AddLogging(l =>
            {
                l.AddConsole().AddDebug().SetMinimumLevel(LogLevel.Debug);
            });
        }

        public override void ConfigureAuthentication(IServiceCollection services, AppConfiguration appConfiguration)
        {
            base.ConfigureAuthentication(services, new AppConfiguration
            {
                Authentication = new AuthenticationConfiguration
                {
                    ServerUri = new Uri("https://mocked-partenaires.local"),
                    LogoutEndpointPath = "/logout",
                    RedirectEndpointPath = "/login",
                    Hangfire = new HangfireAuthenticationConfiguration
                    {
                        SharedSecret = Guid.Empty
                    }
                }
            });
        }

        public override void ConfigureApi(IServiceCollection services)
        {
            services.AddControllers();
        }

        public override void ConfigureNotifications(IServiceCollection services, AppConfiguration configuration)
        {
            base.ConfigureNotifications(services, new AppConfiguration
            {
                Slack = new SlackConfiguration
                {
                    Hooks = new SlackHooks()
                }
            });
        }

        public override void ConfigureCache(IServiceCollection services, AppConfiguration configuration)
        {
            // do not configure redis in a test context
        }

        public override void ConfigureStorage(IServiceCollection services)
        {
            services.AddMockDbContext<IpFilterDbContext>("IpFilters", o => new IpFilterDbContext(o));
            services.AddMockDbContext<UsersDbContext>("Users", o => new UsersDbContext(o));
        }

        public override void ConfigureRights(IServiceCollection services, AppConfiguration configuration)
        {
            base.ConfigureRights(services, new AppConfiguration
            {
                Rights = new RightsConfiguration
                {
                    ServerUri = new Uri("https://mocked-partenaires.local"),
                    DepartmentsEndpointPath = "/api/mocked/Departments",
                    UsersEndpointPath = "/api/mocked/Users",
                    ForeignAppEndpointPath = "/api/mocked/ForeignApp"
                }
            });
        }

        public override void ConfigureEmails(IServiceCollection services, AppConfiguration configuration)
        { }

        public override void ConfigureIpFilter(IServiceCollection services)
        {
            var settings = new LuccaSecuritySettings
            {
                IpWhiteList = new IpWhiteList
                {
                    ResponseStatusCode = 401,
                    AuthorizedIpAddresses = new List<string> { "127.0.0.1", "::1" }
                }
            };
            var options = Options.Create(settings);
            services.AddSingleton(options);

            IpFilterConfigurer.ConfigureServices(services);
        }

        public override void ConfigureSharedDomains(IServiceCollection services, AppConfiguration configuration)
        {
            base.ConfigureSharedDomains(services, new AppConfiguration { Authentication = _mockedAuthConfiguration });
        }

        public override void ConfigureBilling(IServiceCollection services, AppConfiguration configuration)
        {
            base.ConfigureBilling(services, new AppConfiguration
            {
                LegacyCloudControl = new LegacyCloudControlConfiguration
                {
                    Host = "mocked-cc.local"
                },
                BillingContracts = new BillingContractsConfiguration
                {
                    LegacyClientsEndpointPath = "/api/mocked/clients"
                }
            });
        }

        public override void ConfigureHealthCheck(IServiceCollection services, AppConfiguration c)
        {
            services
                .AddHealthCheck(o =>
                        {
                            o.ServiceGuid = new Guid("00000000-0000-0000-0000-000000000000");
                            o.ServiceName = "MOCK";
                        }
                    );
        }

        public override void ConfigureSalesforce(IServiceCollection services, AppConfiguration configuration)
        {
            base.ConfigureSalesforce(services, new AppConfiguration
            {
                Salesforce = new SalesforceConfiguration
                {
                    ServerUri = new Uri("https://mocked-salesforce.local"),
                    AccountsEndpointPath = "/api/mocked/accounts",
                    Token = new Guid("deadbeef-0000-0000-0000-000000000000")
                }
            });
        }

        public override void ConfigureInstances(IServiceCollection services, AppConfiguration configuration)
        {
            base.ConfigureInstances(services, new AppConfiguration
            {
                Instances = new InstancesConfigurer.InstancesConfiguration
                {
                    Identity = new IdentityAuthenticationConfig
                    {
                        ClientId = "mocked.identity.client.id",
                        ClientSecret = "mocked.identity.client.secret",
                        TokenRequestRoute = "mocked/identity/token/request/route"
                    },
                    WsAuth = new WsAuthConfiguration
                    {
                        ServerUri = new Uri("https://mocked-ws-auth.ilucca.local"),
                        EndpointPath = "/sync",
                        Token = new Guid("deadbeef-0000-0000-0000-000000000000")
                    },
                    CcData = new CcDataConfiguration
                    {
                        InboundToken = new Guid("deadbeef-0000-0000-0000-000000000000"),
                        OutboundToken = new Guid("deadbeef-0000-0000-0000-000000000000")
                    },
                    Hubspot = new HubspotConfiguration
                    {
                        OutboundToken = new Guid("deadbeef-0000-0000-0000-000000000000"),
                        ServerUri = new Uri("https://api.hubapi.mocked")
                    },
                    SqlScriptPicker = new SqlScriptPickerConfiguration(),
                    DemoClusterSelection = new ClusterSelectorConfiguration()
                }
            });
        }
    }

    internal static class ServiceCollectionExtensions
    {
        public static void AddMockDbContext<TDbContext>(this IServiceCollection services, string dbName, Func<DbContextOptions<TDbContext>, TDbContext> createDbContext)
            where TDbContext : DbContext
        {
            var dbOptions = new DbContextOptionsBuilder<TDbContext>().UseInMemoryDatabase(dbName).Options;
            services.AddSingleton(createDbContext(dbOptions));
        }
    }
}
