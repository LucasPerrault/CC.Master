using Authentication.Infra.Services;
using Microsoft.AspNetCore.Http;
using System;
using Tools.Web;

namespace Authentication.Web.MagicTokenShame;

public static class HttpRequestExtensions
{
    public static bool IsMagicTokenRequestOnMagicTokenRoute(this HttpContext context, Guid magicToken)
    {
        return context.HasAttribute<AllowMagicTokenAttribute>()
               && CloudControlHeaderTokensReader.TryGetGuidValue(context.Request, "user", out var guid)
               && guid == magicToken;
    }
}
