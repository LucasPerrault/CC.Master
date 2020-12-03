using Authentication.Domain;
using Authentication.Infra.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Authentication.Web.Middlewares
{
    public class UnauthorizedAccessMiddleware
    {
        private static readonly HashSet<string> _anonymousRoutes = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase)
        {
            "/ping",
            "/healthz",
            "/health/ready",
            "/health/live",
            "/warmup"
        };

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
            var isAnonymousRoute = httpContext.Request.Path.HasValue
                                   && _anonymousRoutes.Contains(httpContext.Request.Path.Value);

            if (isAnonymousRoute || HasAttribute<IAllowAnonymous>(httpContext))
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

        private static bool HasAttribute<T>(HttpContext context)
        {
            var endpoint = context.Features?
                .Get<IEndpointFeature>()?
                .Endpoint;

            return endpoint != null && endpoint.Metadata.Any(m => m is T);
        }
    }
}
