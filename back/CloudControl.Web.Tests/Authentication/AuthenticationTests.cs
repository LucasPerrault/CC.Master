using CloudControl.Web.Tests.Mocks;
using Microsoft.AspNetCore.TestHost;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace CloudControl.Web.Tests.Authentication
{
    public class AuthenticationTests
    {
        private readonly HttpClient _client;
        public AuthenticationTests()
        {
            var server = new TestServer(TestHostBuilder<TestUserAuthenticationHandler>.GetInMemory());
            _client = server.CreateClient();
        }

        [Fact]
        public async Task ShouldRedirectSessionKeyRequest()
        {
            var token = "123456789";
            var response = await _client.GetAsync($"/route?sessionKey={token}");
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.Contains($"authToken={token}; path=/", response.Headers.GetValues("Set-Cookie"));
            Assert.Contains("http://localhost/route", response.Headers.GetValues("Location"));
        }

        [Fact]
        public async Task ShouldRedirectLoginRequest()
        {
            var response = await _client.GetAsync("/account/login?returnUrl=%2fdemos");
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.Contains(
                "https://mocked-partenaires.local/login?callback=https://localhost/demos",
                response.Headers.GetValues("Location")
            );
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
