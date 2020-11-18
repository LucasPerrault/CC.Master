using Core.Proxy.Infra.Extensions;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Proxy.Tests
{
    public class RedirectionQualifTests
    {

        [Theory]
        [InlineData("/api/v3/contracts")]
        [InlineData("/billing")]
        [InlineData("/")]
        [InlineData("")]
        public void ShouldRedirectApiCalls(string url)
        {
            var context = new DefaultHttpContext();
            context.Request.Path = url;
            Assert.True(context.IsRedirectableCall());
        }

        [Theory]
        [InlineData("/api/contracts")]
        [InlineData("/api")]
        public void ShouldNotRedirectApiCalls(string url)
        {
            var context = new DefaultHttpContext();
            context.Request.Path = url;
            Assert.False(context.IsRedirectableCall());
        }
    }
}
