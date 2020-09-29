using CloudControl.Web.Tests.Mocks;
using Microsoft.AspNetCore.TestHost;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Authentication.Web.Tests.Controllers
{
    public class LogoutControllerTests
    {
        private readonly HttpClient _client;

        public LogoutControllerTests()
        {
            var server = new TestServer(TestHostBuilder<TestUserAuthenticationHandler>.GetInMemory());
            _client = server.CreateClient();
        }

        [Fact]
        public async Task ShouldRedirectLogoutRequest()
        {
            var response = await _client.GetAsync("/logout");
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.Contains(
                "https://mocked-partenaires.local/logout?callback=https://localhost",
                response.Headers.GetValues("Location")
            );
        }
    }
}
