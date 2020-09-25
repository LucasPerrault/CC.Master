using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System.Net.Http;

namespace CloudControl.Web.Tests.Mocks
{
	public class WebInMemoryDatabaseFixture<TAuthenticationHandler> where TAuthenticationHandler : AuthenticationHandler<TestAuthenticationOptions>
	{
		public readonly HttpClient Client;
		public readonly TestServer Server;

		public WebInMemoryDatabaseFixture()
		{
			IWebHostBuilder host = TestHostBuilder<TAuthenticationHandler>.GetInMemory();
			Server = new TestServer(host);
			Client = Server.CreateClient();
		}
	}
}
