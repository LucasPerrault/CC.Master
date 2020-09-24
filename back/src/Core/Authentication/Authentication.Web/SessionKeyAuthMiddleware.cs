using Authentication.Infra.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using System.Linq;
using System.Threading.Tasks;

namespace Authentication.Web
{
    public class SessionKeyAuthMiddleware
    {
        private const string _authTokenCookieKey = "authToken";
        private readonly RequestDelegate _next;

        public SessionKeyAuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext, SessionKeyService sessionKeyService)
        {
            if (sessionKeyService.ContainsSessionKey(httpContext.Request.Query))
            {
                AuthenticateResponse(httpContext, sessionKeyService);
                return;
            }

            await _next.Invoke(httpContext);
        }

        private void AuthenticateResponse(HttpContext httpContext, SessionKeyService sessionKeyService)
        {
            var token = sessionKeyService.GetSessionKey(httpContext.Request.Query);

            httpContext.Response.Cookies.Append(_authTokenCookieKey, token);

            var redirectionUri = UriHelper.BuildAbsolute(httpContext.Request.Scheme, httpContext.Request.Host, httpContext.Request.PathBase, httpContext.Request.Path);
            httpContext.Response.Redirect(redirectionUri, false);
        }
    }
}
