using Authentication.Infra.Configurations;
using Authentication.Infra.Services;
using CloudControl.Web.Tests.Mocks;
using Moq;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Testing.Infra;
using Xunit;

namespace Authentication.Web.Tests.Controllers
{
    public class LogoutControllerTests
    {

        [Fact]
        public async Task ShouldRedirectLogoutRequest()
        {
            var config = new AuthenticationConfiguration
            {
                ServerUri = new Uri("https://example.org/"),
                LogoutEndpointPath = "log-me-out",
                RedirectEndpointPath = "redirect-user-here",
            };

            var handlerMock = new Mock<HttpClientHandler>();
            handlerMock
                .SetupSendAsync(ItIsRequestMessage.Matching(h => h.RequestUri.ToString() == $"{config.ServerUri}{config.LogoutEndpointPath}"))
                .ReturnsAsync(new HttpResponseMessage());


            var logoutService = new LogoutService(config, TestPrincipal.NewUser(), new HttpClient(handlerMock.Object));
            var authRemoteService = new AuthRedirectionRemoteService(config);

            var webApplicationFactory = new MockedWebApplicationFactory();
            webApplicationFactory.Mocks.AddTransient(logoutService);
            webApplicationFactory.Mocks.AddSingleton(authRemoteService);

            var client = webApplicationFactory.CreateAuthenticatedClient();
            var response = await client.GetAsync("/logout");

            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.Contains
            (
                $"{config.ServerUri}{config.RedirectEndpointPath}?callback=https://localhost",
                response.Headers.GetValues("Location")
            );
        }
    }
}
