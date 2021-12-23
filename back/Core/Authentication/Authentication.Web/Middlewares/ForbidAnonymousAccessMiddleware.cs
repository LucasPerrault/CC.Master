using Authentication.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Tools.Web;

namespace Authentication.Web.Middlewares
{
    public class ForbidAnonymousAccessMiddleware
    {
        private readonly RequestDelegate _next;

        public ForbidAnonymousAccessMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext.HasAttribute<IAllowAnonymous>())
            {
                await _next.Invoke(httpContext);
                return;
            }

            var isAuthenticated = httpContext.User is CloudControlUserClaimsPrincipal or CloudControlApiKeyClaimsPrincipal;

            if (isAuthenticated)
            {
                await _next.Invoke(httpContext);
                return;
            }

            httpContext.Response.StatusCode = 401;
        }
    }
}
