using CloudControl.Web.Tests.Mocks;
using Microsoft.AspNetCore.TestHost;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Authentication.Web.Tests.Middlewares
{
    public class SessionKeyAuthMiddlewareTests
    {
        private readonly HttpClient _client;
        public SessionKeyAuthMiddlewareTests()
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
            Assert.Contains("https://localhost/route", response.Headers.GetValues("Location"));
        }
    }
}
