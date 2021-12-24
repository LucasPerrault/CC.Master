using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tools.Web;

public static class HttpRequestExtensions
{
    private const string ApiRoutePrefix = "/api";

    private static HashSet<string> WhitelistedRoutes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "/ping",
        "/healthz",
        "/health/ready",
        "/health/live",
        "/warmup",
        "/maj",
    };
    private static HashSet<string> WhitelistedRoutePrefixes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "/account",

        // needed by front
        "/api/v3/principals/me",
        "/api/principals/me",

        // front
        "/ip",
        "/invalid-email-domain",
        "/cc-master",
    };

    public static bool IsRouteWhitelistedForRestrictions(this HttpRequest request)
    {
        return WhitelistedRoutes.Contains(request.Path)
               || WhitelistedRoutePrefixes.Any(prefix => request.Path.StartsWithSegments(prefix));
    }

    public static bool IsApiCall(this HttpRequest request) => request.Path.Value.StartsWith(ApiRoutePrefix);
}
