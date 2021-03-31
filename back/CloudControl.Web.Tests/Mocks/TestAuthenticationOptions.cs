using Microsoft.AspNetCore.Authentication;
using System;
using System.Security.Claims;

namespace CloudControl.Web.Tests.Mocks
{
    public class TestAuthenticationOptions : AuthenticationSchemeOptions
    {
        public virtual ClaimsIdentity Identity { get; } = new ClaimsIdentity(new[] {
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
        }, "TEST");
    }
}
