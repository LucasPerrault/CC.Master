using Authentication.Domain;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace CloudControl.Web.Tests.Mocks
{
	public class TestApiKeyAuthenticationHandler : AuthenticationHandler<TestAuthenticationOptions>
	{
		public TestApiKeyAuthenticationHandler(
			IOptionsMonitor<TestAuthenticationOptions> options,
			ILoggerFactory logger,
			UrlEncoder encoder,
			ISystemClock clock
		) : base(options, logger, encoder, clock)
		{ }

		protected override Task<AuthenticateResult> HandleAuthenticateAsync()
		{
			var authenticationTicket = new AuthenticationTicket(new CloudControlApiKeyClaimsPrincipal(new ApiKey
			{
				Name = "Mocked api key",
				Token = Guid.NewGuid(),
			}), new AuthenticationProperties(), "TEST");

			return Task.FromResult(AuthenticateResult.Success(authenticationTicket));
		}
	}
}
