using Authentication.Domain;
using Distributors.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tools.Web;

namespace Distributors.Web;

public static class ApplicationBuilderExtensions
{
    public static void UseDistributorDomainFilter(this IApplicationBuilder app)
    {
        app.UseWhen
        (
            c => !c.Request.IsRouteWhitelistedForRestrictions(),
            app => app.UseMiddleware<DistributorDomainRestrictionMiddleware>()
        );
    }
}

public class DistributorDomainRestrictionMiddleware
{
    private readonly RequestDelegate _next;

    public DistributorDomainRestrictionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext httpContext, IDistributorDomainService service, DistributorsConfiguration configuration)
    {
        if (!configuration.ShouldFilterDistributorDomains)
        {
            await _next.Invoke(httpContext);
            return;
        }

        if (httpContext.User is not CloudControlUserClaimsPrincipal user)
        {
            await _next.Invoke(httpContext);
            return;
        }

        var domain = service.GetDomain(user.User.Mail);
        if (await service.IsRegisteredAsync(domain, user.User.DistributorId))
        {
            await _next.Invoke(httpContext);
            return;
        }

        if (httpContext.Request.IsApiCall())
        {
            httpContext.Response.StatusCode = 401;
            return;
        }

        var registered = await service.GetAllRegistered(user.User.DistributorId);
        httpContext.Response.StatusCode = 302;
        var baseAddress = new Uri($"https://{httpContext.Request.Host.Value}");
        httpContext.Response.Headers.Location = new StringValues(GetRedirectionLocation(baseAddress, domain, registered).ToString());
    }

    private Uri GetRedirectionLocation(Uri baseAddress, Domain.Domain domain, List<Domain.Domain> registered)
    {
        return new Uri(baseAddress, $"/invalid-email-domain?domain={domain.Value}&registered={string.Join(',', registered.Select(r => r.Value))}");
    }
}
