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
            var handlerMock = new Mock<HttpClientHandler>();
            handlerMock
                .SetupSendAsync(ItIsRequestMessage.Matching(h => h.RequestUri.ToString() == "https://mocked-partenaires.mocked/log-me-out"))
                .ReturnsAsync(new HttpResponseMessage());

            var config = new AuthenticationConfiguration
            {
                ServerUri = new Uri("https://mocked-partenaires.mocked/"),
                LogoutEndpointPath = "log-me-out",
                RedirectEndpointPath = "redirect-user-here",
            };

            var logoutService = new LogoutService(config, new TestPrincipal().Principal, new HttpClient(handlerMock.Object));
            var authRemoteService = new AuthRedirectionRemoteService(config);

            var webApplicationFactory = new MockedWebApplicationFactory();
            webApplicationFactory.AddTransient(logoutService);
            webApplicationFactory.AddSingleton(authRemoteService);

            var client = webApplicationFactory.CreateAuthenticatedClient();
            var response = await client.GetAsync("/logout");

            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.Contains
            (
                "https://mocked-partenaires.mocked/redirect-user-here?callback=https://localhost",
                response.Headers.GetValues("Location")
            );
        }
    }
}
