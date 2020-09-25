using Authentication.Infra.Services;
using Microsoft.AspNetCore.Mvc;

namespace Authentication.Web.Controllers
{
    [Route("Account")]
    public class AccountController : Controller
    {
        private readonly AuthRedirectionRemoteService _authRedirectionRemoteService;

        public AccountController
        (
            AuthRedirectionRemoteService authRedirectionRemoteService
        )
        {
            _authRedirectionRemoteService = authRedirectionRemoteService;
        }

        [HttpGet, Route("Login")]
        public RedirectResult Login([FromQuery] string returnUrl)
        {
            var redirectionCallback = $"{Request.Host.Value}{returnUrl}";
            var authUrl = _authRedirectionRemoteService.GetAuthRedirectionUri(redirectionCallback);

            return Redirect(authUrl.ToString());
        }
    }
}
