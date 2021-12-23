using Authentication.Infra.Configurations;
using Cache.Abstractions;
using CloudControl.Web.Configuration;
using CloudControl.Web.Tests.Mocks.Overrides;
using Email.Domain;
using IpFilter.Domain;
using IpFilter.Web;
using Lock;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Resources.Translations;
using Rights.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TeamNotification.Abstractions;
using Users.Domain;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace CloudControl.Web.Tests.Mocks
{
    public class MockedWebApplicationFactory : WebApplicationFactory<Startup>
    {
        public MockedWebApplicationMocks Mocks { get; } = new MockedWebApplicationMocks();
        public MockedWebApplicationConfiguration Config { get; } = new MockedWebApplicationConfiguration();

        public MockedWebApplicationFactory()
        {
            Mocks.AddSingleton(new MockMiddlewareConfig());
        }

        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            return WebHost.CreateDefaultBuilder(null)
                .UseEnvironment("Development")
                .ConfigureServices(s =>
                {
                    s.AddSingleton<ServicesConfiguration>(sp => new MockedServicesConfiguration
                    (
                        Config,
                        sp.GetRequiredService<IConfiguration>(),
                        sp.GetRequiredService<IWebHostEnvironment>()
                    ));
                })
                .ConfigureTestServices(Mocks.ConfigureAdditionalServices)
                .UseStartup<Startup>()
                .UseSetting(WebHostDefaults.ApplicationKey, typeof(Startup).Assembly.GetName().Name)
                .UseTestServer();
        }

        public HttpClient CreateAuthenticatedClient()
        {
            var httpClient = CreateClient();
            var fakeUserToken = "5b2c0440-2acf-443a-9b1d-3a2a1e09dad4";
            httpClient.DefaultRequestHeaders.Authorization = System.Net.Http.Headers.AuthenticationHeaderValue.Parse($"Cloudcontrol user={fakeUserToken}");
            return httpClient;
        }


        public new HttpClient CreateClient()
        {
            return CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        }
    }

    public class MockedServicesConfiguration : ServicesConfiguration
    {
        private readonly MockedWebApplicationConfiguration _mockedConfig;

        public MockedServicesConfiguration(MockedWebApplicationConfiguration mockedConfig, IConfiguration configuration, IWebHostEnvironment env)
            : base(configuration, env)
        {
            _mockedConfig = mockedConfig;
        }

        public override AppConfiguration ConfigureConfiguration(IServiceCollection services)
        {
            return null;
        }

        public override void ConfigureHealthCheck(IServiceCollection services, AppConfiguration configuration)
        {
            services.AddHealthChecks();
        }

        public override void ConfigureCache(IServiceCollection services, AppConfiguration configuration)
        {
            services.AddScoped(_ => new Mock<ICacheService>().Object);
        }

        public override void ConfigureLock(IServiceCollection services, AppConfiguration configuration)
        {
            services.AddSingleton(new Mock<ILockService>().Object);
        }

        public override void ConfigureIpFilter(IServiceCollection services, AppConfiguration configuration)
        {
            var options = Options.Create(_mockedConfig.LuccaSecuritySettings);
            services.AddSingleton(options);

            IpFilterConfigurer.ConfigureServices(services);

            var ipFilterAuthorizationMock = new Mock<IIpFilterAuthorizationStore>();
            ipFilterAuthorizationMock
                .Setup(i => i.GetAsync(It.IsAny<IpFilterAuthorizationFilter>()))
                .ReturnsAsync(new List<IpFilterAuthorization>());
            services.AddSingleton(ipFilterAuthorizationMock.Object);


            var authRequestStore = new Mock<IIpFilterAuthorizationRequestStore>();
            authRequestStore
                .Setup(s => s.FirstOrDefaultAsync(It.IsAny<IpFilterAuthorizationRequestFilter>()))
                .ReturnsAsync((IpFilterAuthorizationRequest)null);
            services.AddScoped(_ => authRequestStore.Object);
            services.AddScoped(_ => new Mock<IIpFilterTranslations>().Object);
        }

        public override void ConfigureNotifications(IServiceCollection services, AppConfiguration configuration)
        {
            services.AddSingleton(new Mock<ITeamNotifier>().Object);
        }

        public override void ConfigureStorage(IServiceCollection services)
        {
        }

        public override void ConfigureSharedDomains(IServiceCollection services, AppConfiguration configuration)
        {
        }

        public override void ConfigureAuthentication(IServiceCollection services, AppConfiguration configuration)
        {
            var usersServiceMock = new Mock<IUsersService>();
            usersServiceMock
                .Setup(u => u.GetByTokenAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new User());
            services.AddScoped(_ => usersServiceMock.Object);
            base.ConfigureAuthentication(services, new AppConfiguration
            {
                Authentication = new AuthenticationConfiguration
                {
                    ServerUri = new Uri("https://mocked-partenaires.local"),
                    LogoutEndpointPath = "/logout",
                    RedirectEndpointPath = "/login",
                }
            });
        }

        public override void ConfigureEmails(IServiceCollection services, AppConfiguration configuration)
        {
            services.AddScoped(_ => new Mock<IEmailService>().Object);
        }

        public override void ConfigureRights(IServiceCollection services, AppConfiguration configuration)
        {
            var rightsServiceMock = new Mock<IRightsService>();
            services.AddSingleton(rightsServiceMock.Object);
        }

        public override void ConfigureBilling(IServiceCollection services, AppConfiguration configuration)
        {
        }

        public override void ConfigureInstances(IServiceCollection services, AppConfiguration configuration)
        {
        }

        public override void ConfigureLogs(IServiceCollection services)
        {
            services.AddLogging(l =>
            {
                l.AddConsole().AddDebug().SetMinimumLevel(LogLevel.Debug);
            });
        }

        public override void ConfigureSalesforce(IServiceCollection services, AppConfiguration configuration)
        { }

        public override void ConfigureAdvancedFilters(IServiceCollection services, AppConfiguration configuration)
        { }

        public override void ConfigureSlack(IServiceCollection services, AppConfiguration configuration)
        { }
    }

    public static class ApiTestExtensions
    {
        public static async Task<HttpResponseMessage> CatchApplicationErrorBody(this Task<HttpResponseMessage> task)
        {
            var response = await task;
            if (response.StatusCode != HttpStatusCode.InternalServerError)
            {
                return response;
            }

            var errorBody = await response.Content.ReadAsStringAsync();
            throw new Exception(errorBody);
        }
    }
}
