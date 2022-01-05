using Authentication.Domain;
using IpFilter.Domain;
using Lucca.Core.AspNetCore.Middlewares;
using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace IpFilter.Web
{
    public class IpRejectionService : IIpRejectionService
    {
        private readonly IpFilterRequestCreationService _requestCreationService;
        private readonly ClaimsPrincipal _principal;

        public IpRejectionService
        (
            ClaimsPrincipal principal,
            IpFilterRequestCreationService requestCreationService
        )
        {
            _principal = principal;
            _requestCreationService = requestCreationService;
        }

        public async Task HandleRejectionAsync(IpRejection rejection, HttpContext httpContext)
        {
            if (_principal is not CloudControlUserClaimsPrincipal user)
            {
                httpContext.Response.StatusCode = 403;
                return;
            }

            var baseAddress = new Uri($"https://{httpContext.Request.Host.Value}");
            httpContext.Response.Redirect(GetRedirectionLocation(baseAddress).ToString());

            var rejectedUser = new RejectedUser
            {
                IpFilterUser = new IpFilterUser {IpAddress = rejection.Address.ToString(), UserId = user.UserId.Value},
                FirstName = user.User.FirstName,
                LastName = user.User.LastName,
            };

            var redirection = $"{httpContext.Request.Path}{httpContext.Request.QueryString}";
            await _requestCreationService.SendRequestIfNeededAsync(rejectedUser, EmailHrefBuilder(baseAddress, redirection));
        }

        private Uri GetRedirectionLocation(Uri baseAddress)
        {
            return new Uri(baseAddress, "/ip");
        }

        private EmailHrefBuilder EmailHrefBuilder(Uri baseAddress, string redirection) => new EmailHrefBuilder
        {
            Accept = guid => new Uri(baseAddress, $"/ip/confirm?code={guid}&redirection={HttpUtility.UrlEncode(redirection)}").ToString(),
            Reject = guid => new Uri(baseAddress, $"/ip/reject?code={guid}").ToString(),
        };
    }
}
