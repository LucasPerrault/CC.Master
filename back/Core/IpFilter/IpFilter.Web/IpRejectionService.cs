using Authentication.Domain;
using IpFilter.Domain;
using Lucca.Core.AspNetCore.Middlewares;
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

        public Task HandleRejectionAsync(IpRejection rejection)
        {
            if (_principal is not CloudControlUserClaimsPrincipal user)
            {
                return Task.CompletedTask;
            }

            var rejectedUser = new RejectedUser
            {
                IpFilterUser = new IpFilterUser {IpAddress = rejection.Address.ToString(), UserId = user.UserId.Value},
                FirstName = user.User.FirstName,
                LastName = user.User.LastName,
            };

            return _requestCreationService.SendRequestIfNeededAsync(rejectedUser, EmailHrefBuilder);
        }

        private EmailHrefBuilder EmailHrefBuilder => new EmailHrefBuilder
        {
            Accept = guid => new Uri(_configuration.CloudControlBaseAddress, $"/ip/confirm?code={guid}").ToString(),
            Reject = guid => new Uri(_configuration.CloudControlBaseAddress, $"/ip/reject?code={guid}").ToString(),
        };
    }
}
