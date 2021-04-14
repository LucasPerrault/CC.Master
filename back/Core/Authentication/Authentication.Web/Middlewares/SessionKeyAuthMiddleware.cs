using Authentication.Infra.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using System.Threading.Tasks;

namespace Authentication.Web.Middlewares
{
    public class SessionKeyAuthMiddleware
    {
        private readonly RequestDelegate _next;

        public SessionKeyAuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke
        (
            HttpContext httpContext,
            SessionKeyService sessionKeyService,
            AuthTokenCookieService cookieService
        )
        {
            if (sessionKeyService.ContainsSessionKey(httpContext.Request.Query))
            {
                AuthenticateResponse(httpContext, sessionKeyService, cookieService);
                return;
            }

            await _next.Invoke(httpContext);
        }

        private void AuthenticateResponse
        (
            HttpContext httpContext,
            SessionKeyService sessionKeyService,
            AuthTokenCookieService cookieService
        )
        {
            var token = sessionKeyService.GetSessionKey(httpContext.Request.Query);

            cookieService.SetAuthTokenCookie(httpContext, token);

            var redirectionUri = UriHelper.BuildAbsolute
                (
                    "https",
                    httpContext.Request.Host,
                    httpContext.Request.PathBase,
                    httpContext.Request.Path
                );
            httpContext.Response.Redirect(redirectionUri, false);
        }
    }
}
