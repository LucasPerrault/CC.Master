using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using System.Linq;

namespace Tools
{
    public static class HttpContextExtensions
    {
        public static bool HasAttribute<T>(this HttpContext context)
        {
            var endpoint = context.Features?
                .Get<IEndpointFeature>()?
                .Endpoint;

            return endpoint != null && endpoint.Metadata.Any(m => m is T);
        }
    }
}
