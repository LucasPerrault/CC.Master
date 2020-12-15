using Authentication.Infra.Services;
using IpFilter.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Authentication.Web.Controllers
{
    [Route("Account")]
    [AllowAnonymous]
    [AllowAllIps]
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
            var redirectionCallback = $"https://{Request.Host.Value}{returnUrl}";
            var authUrl = _authRedirectionRemoteService.GetAuthRedirectionUri(redirectionCallback);

            return Redirect(authUrl.ToString());
        }
    }
}
