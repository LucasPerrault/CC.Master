using Core.Proxy.Infra.Extensions;
using Microsoft.AspNetCore.Http;
using Rights.Web;
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

        [Theory]
        [InlineData("/contracts")]
        public void ShouldNotRedirectForBetaTesters(string url)
        {
            var context = new DefaultHttpContext();
            Assert.True(context.ShouldRedirect());

            BetaTesterHelper.SetBetaTester(context, true);
            context.Request.Path = url;
            Assert.False(context.ShouldRedirect());
        }
    }
}
