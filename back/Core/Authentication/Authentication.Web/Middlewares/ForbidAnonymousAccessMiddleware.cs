using Authentication.Domain;
using Authentication.Infra.Configurations;
using Authentication.Web.MagicTokenShame;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Tools.Web;

namespace Authentication.Web.Middlewares
{
    public class ForbidAnonymousAccessMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly AuthenticationConfiguration _configuration;

        public ForbidAnonymousAccessMiddleware(RequestDelegate next, AuthenticationConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
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

            if (httpContext.IsMagicTokenRequestOnMagicTokenRoute(_configuration.MagicToken))
            {
                await _next.Invoke(httpContext);
                return;
            }

            httpContext.Response.StatusCode = 401;
        }
    }
}
