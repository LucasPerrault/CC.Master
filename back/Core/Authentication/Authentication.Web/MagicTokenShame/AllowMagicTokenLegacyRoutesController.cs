using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;

namespace Authentication.Web.MagicTokenShame;

[AllowMagicToken]
public class AllowMagicTokenLegacyRoutesController
{

    [HttpGet("api/v3/instances")]
    [HttpPost("/api/v3/environments/markTrainingRestorationAsEnded")]
    public HttpResponseMessage AllowMagicTokenCallToLegacy()
        => throw new ApplicationException("Calls to this route should be intercepted by proxy");
}
