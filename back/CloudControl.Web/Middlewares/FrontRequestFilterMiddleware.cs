using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CloudControl.Web.Middlewares
{
    public class FrontRequestFilterMiddleware
    {
        private static readonly HashSet<string> _nonFrontRequestPrefixes = new HashSet<string>(StringComparer.OrdinalIgnoreCase) {  "/api" };

        private readonly RequestDelegate _next;

        public FrontRequestFilterMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var isFrontRequest = !httpContext.Request.Path.HasValue
                || !_nonFrontRequestPrefixes.Any(prefix => httpContext.Request.Path.Value.StartsWith(prefix));

            if (!isFrontRequest)
            {
                httpContext.Response.StatusCode = 404;
                return;
            }

            await _next.Invoke(httpContext);
        }
    }
}
