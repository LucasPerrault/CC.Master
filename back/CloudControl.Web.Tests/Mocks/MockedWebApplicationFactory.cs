using Authentication.Infra.Configurations;
using CloudControl.Web.Configuration;
using IpFilter.Domain;
using IpFilter.Web;
using Lucca.Core.AspNetCore.Middlewares;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Rights.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Net.Http;
using Users.Domain;

namespace CloudControl.Web.Tests.Mocks
{

    public class MockedWebApplicationFactory : WebApplicationFactory<Startup>
    {
        public MockedWebApplicationMocks Mocks { get; } = new MockedWebApplicationMocks();

        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            return WebHost.CreateDefaultBuilder(null)
                .UseEnvironment("Development")
                .ConfigureServices(s =>
                {
                    s.AddSingleton<ServicesConfiguration>(sp => new MockedServicesConfiguration
                    (
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
        public MockedServicesConfiguration(IConfiguration configuration, IWebHostEnvironment env)
            : base(configuration, env)
        { }

        public override AppConfiguration ConfigureConfiguration(IServiceCollection services)
        {
            return null;
        }

        public override void ConfigureHealthCheck(IServiceCollection services, AppConfiguration configuration)
        {
            services.AddHealthChecks();
        }

        public override void ConfigureCache(IServiceCollection services, AppConfiguration configuration)
        { }

        public override void ConfigureLock(IServiceCollection services, AppConfiguration configuration)
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

            var ipFilterAuthorizationMock = new Mock<IIpFilterAuthorizationStore>();
            ipFilterAuthorizationMock
                .Setup(i => i.GetByUserAsync(It.IsAny<IpFilterUser>()))
                .ReturnsAsync(new List<IpFilterAuthorization>());
            services.AddSingleton(ipFilterAuthorizationMock.Object);
        }

        public override void ConfigureNotifications(IServiceCollection services, AppConfiguration configuration)
        {
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
    }
}
