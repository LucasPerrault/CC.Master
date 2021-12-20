using Authentication.Domain;
using IpFilter.Domain;
using Lucca.Core.AspNetCore.Middlewares;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IpFilter.Web
{
    public class IpRejectionService : IIpRejectionService
    {
        private readonly IpFilterRequestCreationService _requestCreationService;
        private readonly IpFilterConfiguration _configuration;
        private readonly ClaimsPrincipal _principal;

        public IpRejectionService
        (
            ClaimsPrincipal principal,
            IpFilterRequestCreationService requestCreationService,
            IpFilterConfiguration configuration
        )
        {
            _principal = principal;
            _requestCreationService = requestCreationService;
            _configuration = configuration;
        }

        public async Task HandleRejectionAsync(IpRejection rejection, HttpContext httpContext)
        {
            if (_principal is not CloudControlUserClaimsPrincipal user)
            {
                httpContext.Response.StatusCode = 403;
                return;
            }

            httpContext.Response.StatusCode = 302;
            httpContext.Response.Headers.Location = new StringValues(GetRedirectionLocation().ToString());

            var rejectedUser = new RejectedUser
            {
                IpFilterUser = new IpFilterUser {IpAddress = rejection.Address.ToString(), UserId = user.UserId.Value},
                FirstName = user.User.FirstName,
                LastName = user.User.LastName,
            };

            await _requestCreationService.SendRequestIfNeededAsync(rejectedUser, EmailHrefBuilder);
        }

        private Uri GetRedirectionLocation()
        {
            return new Uri(_configuration.CloudControlBaseAddress, "/ip");
        }

        private EmailHrefBuilder EmailHrefBuilder => new EmailHrefBuilder
        {
            Accept = guid => new Uri(_configuration.CloudControlBaseAddress, $"/ip/confirm?code={guid}").ToString(),
            Reject = guid => new Uri(_configuration.CloudControlBaseAddress, $"/ip/reject?code={guid}").ToString(),
        };
    }
}
