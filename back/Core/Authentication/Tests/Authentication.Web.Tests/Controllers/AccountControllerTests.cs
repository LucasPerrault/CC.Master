using CloudControl.Web.Tests.Mocks;
using Microsoft.AspNetCore.TestHost;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Authentication.Web.Tests.Controllers
{
    public class AccountControllerTests
    {
        private readonly HttpClient _client;

        public AccountControllerTests()
        {
            var server = new MockedWebApplicationFactory();
            _client = server.CreateClient();
        }

        [Fact]
        public async Task ShouldRedirectLoginRequest()
        {
            var response = await _client.GetAsync("/account/login?returnUrl=%2fdemos");
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.Contains
            (
                "https://mocked-partenaires.local/login?callback=https://localhost/demos",
                response.Headers.GetValues("Location")
            );
        }
    }
}
