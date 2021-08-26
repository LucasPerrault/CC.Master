using Authentication.Infra.Services;
using IpFilter.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Authentication.Web.Controllers
{
    [Route("Logout")]
    [AllowAnonymous]
    [AllowAllIps]
    public class LogoutController : Controller
    {
        private readonly AuthRedirectionRemoteService _authRedirectionRemoteService;
        private readonly LogoutService _logoutService;
        private readonly AuthTokenCookieService _cookieService;

        public LogoutController
        (
            AuthRedirectionRemoteService authRedirectionRemoteService,
            LogoutService logoutService,
            AuthTokenCookieService cookieService
        )
        {
            _authRedirectionRemoteService = authRedirectionRemoteService;
            _logoutService = logoutService;
            _cookieService = cookieService;
        }

        [HttpGet]
        public async Task<RedirectResult> Logout()
        {
            _cookieService.InvalidateAuthTokenCookie(HttpContext);
            var redirectionCallback = $"https://{Request.Host.Value}";
            await _logoutService.LogoutAsync();
            var authUrl = _authRedirectionRemoteService.GetAuthRedirectionUri(redirectionCallback);

            return Redirect(authUrl.ToString());
        }
    }
}
