using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;

namespace CloudControl.Web.Controllers;

[AllowAnonymous]
public class AnonymousLegacyRoutesController
{
    [HttpGet("/maj")]
    [HttpPost("api/v3/opportunitiesSync")]
    public HttpResponseMessage AllowAnonymousCallToLegacy()
        => throw new ApplicationException("Calls to this route should be intercepted by proxy");
}
