using Authentication.Domain;
using Authentication.Infra.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using System.Threading.Tasks;
using Tools.Web;

namespace Authentication.Web.Middlewares
{
    public class UnauthorizedAccessMiddleware
    {
        private static readonly string _apiRoutePrefix = "/api";

        private readonly RequestDelegate _next;
        private readonly AuthRedirectionRemoteService _redirectionService;

        public UnauthorizedAccessMiddleware(RequestDelegate next, AuthRedirectionRemoteService redirectionService)
        {
            _next = next;
            _redirectionService = redirectionService;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext.HasAttribute<IAllowAnonymous>())
            {
                await _next.Invoke(httpContext);
                return;
            }

            var isAuthenticated = httpContext.User is CloudControlUserClaimsPrincipal || httpContext.User is CloudControlApiKeyClaimsPrincipal;

            if (!isAuthenticated)
            {
                HandleUnauthenticatedRequest(httpContext);
                return;
            }

            await _next.Invoke(httpContext);
        }

        private void HandleUnauthenticatedRequest(HttpContext httpContext)
        {
            var isApiCall = httpContext.Request.Path.HasValue && httpContext.Request.Path.Value.StartsWith(_apiRoutePrefix);
            if (!isApiCall)
            {
                var callbackUrl = UriHelper.GetEncodedUrl(httpContext.Request);
                var redirectionUri = _redirectionService.GetAuthRedirectionUri(callbackUrl);
                httpContext.Response.Redirect(redirectionUri.ToString(), false);
            }
            else
            {
                httpContext.Response.StatusCode = 401;
            }
        }
    }
}
