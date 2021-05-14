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
        [InlineData("/api/workerprocesses/blablabla")]
        [InlineData("/")]
        [InlineData("")]
        public void ShouldRedirect(string url)
        {
            var context = new DefaultHttpContext();
            context.Request.Path = url;
            Assert.True(context.ShouldRedirect());
        }

        [Theory]
        [InlineData("/account/login")]
        [InlineData("/logout")]
        [InlineData("/api/notALegacyV3Segment")]
        [InlineData("/api")]
        [InlineData("/ping")]
        [InlineData("/healthz")]
        [InlineData("/logs")]
        public void ShouldNotRedirect(string url)
        {
            var context = new DefaultHttpContext();
            context.Request.Path = url;
            Assert.False(context.ShouldRedirect());
        }
    }
}
