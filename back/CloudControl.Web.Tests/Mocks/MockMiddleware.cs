using Microsoft.AspNetCore.Http;
using System.Net;
using System.Threading.Tasks;

namespace CloudControl.Web.Tests.Mocks.Overrides
{
    public class MockMiddlewareConfig
    {
        public IPAddress RemoteIpAddress { get; set; } = null;
    }

    public class MockMiddleware
    {
        private readonly RequestDelegate next;
        private readonly MockMiddlewareConfig _config;

        public MockMiddleware(RequestDelegate next, MockMiddlewareConfig config)
        {
            this.next = next;
            _config = config;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            httpContext.Connection.RemoteIpAddress = _config.RemoteIpAddress;

            await this.next(httpContext);
        }
    }
}
