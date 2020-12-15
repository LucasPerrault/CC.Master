using Authentication.Infra.Services;
using IpFilter.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Authentication.Web.Controllers
{
    [Route("Logout")]
    [AllowAnonymous]
    [AllowAllIps]
    public class LogoutController : Controller
    {
        private readonly AuthRedirectionRemoteService _authRedirectionRemoteService;
        private readonly AuthTokenCookieService _cookieService;

        public LogoutController
        (
            AuthRedirectionRemoteService authRedirectionRemoteService,
            AuthTokenCookieService cookieService
        )
        {
            _authRedirectionRemoteService = authRedirectionRemoteService;
            _cookieService = cookieService;
        }

        [HttpGet]
        public RedirectResult Logout()
        {
            _cookieService.InvalidateAuthTokenCookie(HttpContext);
            var redirectionCallback = $"https://{Request.Host.Value}";
            var authUrl = _authRedirectionRemoteService.GetLogoutRedirectionUri(redirectionCallback);

            return Redirect(authUrl.ToString());
        }
    }
}
