using Authentication.Domain;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace CloudControl.Web.Tests.Mocks
{
	public class TestUserAuthenticationHandler : AuthenticationHandler<TestAuthenticationOptions>
	{
		public TestUserAuthenticationHandler(
			IOptionsMonitor<TestAuthenticationOptions> options,
			ILoggerFactory logger,
			UrlEncoder encoder,
			ISystemClock clock
		) : base(options, logger, encoder, clock)
		{
		}

		protected override Task<AuthenticateResult> HandleAuthenticateAsync()
		{
			var authenticationTicket = new AuthenticationTicket(new CloudControlUserClaimsPrincipal(new Principal
			{
				UserId = 23,
				User = new User
                {
                    FirstName = "Bernard",
                    LastName = "Martin"
                }
			}), new AuthenticationProperties(), "TEST");

			return Task.FromResult(AuthenticateResult.Success(authenticationTicket));
		}
	}
}
