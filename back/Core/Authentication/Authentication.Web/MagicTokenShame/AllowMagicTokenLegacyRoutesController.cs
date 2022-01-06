using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;

namespace Authentication.Web.MagicTokenShame;

[AllowMagicToken]
public class AllowMagicTokenLegacyRoutesController
{

    [HttpGet("api/v3/instances")]
    public HttpResponseMessage AllowMagicTokenCallToLegacy()
        => throw new ApplicationException("Calls to this route should be intercepted by proxy");
}
