using Lucca.Core.AspNetCore.Middlewares;
using System.Collections.Generic;

namespace CloudControl.Web.Tests.Mocks;

public class MockedWebApplicationConfiguration
{
    public LuccaSecuritySettings LuccaSecuritySettings { get; set; } = new LuccaSecuritySettings
    {
        IpWhiteList = new IpWhiteList
        {
            ResponseStatusCode = 401,
            AuthorizedIpAddresses = new List<string> { "127.0.0.1", "::1" },
        },
    };
}
