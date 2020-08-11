using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using System.Linq;
using System.Threading.Tasks;

namespace CloudControl.Web.Middlewares
{
    public class SessionKeyAuthMiddleware
    {
        private const string _sessionKeyKey = "sessionKey";
        private const string _authTokenCookieKey = "authToken";
        private readonly RequestDelegate _next;

        public SessionKeyAuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var hasSessionKeyParam = httpContext.Request.Query.ContainsKey(_sessionKeyKey);
            if (hasSessionKeyParam)
            {
                AuthenticateResponse(httpContext);
                return;
            }

            await _next.Invoke(httpContext);
        }

        private void AuthenticateResponse(HttpContext httpContext)
        {
            var token = httpContext.Request.Query[_sessionKeyKey].Single();

            httpContext.Response.Cookies.Append(_authTokenCookieKey, token);

            var redirectionUri = UriHelper.BuildAbsolute(httpContext.Request.Scheme, httpContext.Request.Host, httpContext.Request.PathBase, httpContext.Request.Path);
            httpContext.Response.Redirect(redirectionUri, false);
        }
    }
}
