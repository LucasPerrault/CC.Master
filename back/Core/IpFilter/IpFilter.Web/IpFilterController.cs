using Authentication.Domain;
using IpFilter.Domain;
using IpFilter.Domain.Accessors;
using Lucca.Core.Shared.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IpFilter.Web
{

    [ApiController, Route("/ip-filter")]
    [AllowAllIps]
    public class IpFilterController : ControllerBase
    {
        private readonly IIpAccessor _ipAccessor;
        private readonly IpFilterService _service;
        private readonly ClaimsPrincipal _principal;

        public IpFilterController(IIpAccessor ipAccessor, IpFilterService service, ClaimsPrincipal principal)
        {
            _ipAccessor = ipAccessor;
            _service = service;
            _principal = principal;
        }

        [HttpGet("validity")]
        public async Task<RequestValidity> GetValidityAsync([FromQuery]ValidityQueryParams queryParams)
        {
            var ipUser = GetUser();
            var expiresAt = await _service.GetRequestExpirationAsync(ipUser, queryParams.Code);
            return new RequestValidity { ExpiresAt = expiresAt };
        }

        public class RequestValidity
        {
            public DateTime ExpiresAt { get; set; }
        }

        public class ValidityQueryParams
        {
            public Guid Code { get; set; }
        }

        [HttpPost("confirm")]
        public async Task<IActionResult> AuthorizeAsync([FromBody]ConfirmationBody queryParams)
        {
            var ipUser = GetUser();
            await _service.ConfirmAsync(ipUser, queryParams.Code, queryParams.Duration);
            return Ok();
        }

        [HttpPost("reject")]
        public async Task<IActionResult> RejectAsync([FromBody]RejectionBody queryParams)
        {
            var ipUser = GetUser();
            await _service.RejectAsync(ipUser, queryParams.Code);
            return Ok();
        }

        public class RejectionBody
        {
            public Guid Code { get; set; }
        }

        public class ConfirmationBody
        {
            public Guid Code { get; set; }
            public AuthorizationDuration Duration { get; set; }
        }

        private IpFilterUser GetUser()
        {
            if (_principal is not CloudControlUserClaimsPrincipal user)
            {
                throw new BadRequestException("Ips can be authorized for users only");
            }

            return new IpFilterUser
            {
                UserId = user.UserId.Value,
                IpAddress = _ipAccessor.IpAddress.ToString(),
            };
        }
    }
}
