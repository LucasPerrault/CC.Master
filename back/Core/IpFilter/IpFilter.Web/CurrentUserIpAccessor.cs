﻿using Authentication.Domain;
using IpFilter.Domain;
using Lucca.Core.AspNetCore.Middlewares;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IpFilter.Web
{
    public class CurrentUserIpAccessor : IScopedIpWhitelistAccessor
    {
        private static readonly HashSet<IPAddress>_emptyResponse = new HashSet<IPAddress>();
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ClaimsPrincipal _principal;
        private readonly IIpFilterService _ipFilterService;

        public CurrentUserIpAccessor(IHttpContextAccessor httpContextAccessor, ClaimsPrincipal principal, IIpFilterService ipFilterService)
        {
            _httpContextAccessor = httpContextAccessor;
            _principal = principal;
            _ipFilterService = ipFilterService;
        }

        private HttpContext HttpContext => _httpContextAccessor.HttpContext;

        public async Task<HashSet<IPAddress>> GetIpWhitelistAsync()
        {
            if (!(_principal is CloudControlUserClaimsPrincipal userPrincipal) || !userPrincipal.UserId.HasValue)
            {
                return _emptyResponse;
            }

            var ip = HttpContext.Connection?.RemoteIpAddress;
            if (ip == null)
            {
                return _emptyResponse;
            }

            var user = new IpFilterUser
            {
                IpAddress = ip.ToString(),
                UserId = userPrincipal.UserId.Value
            };

            var validAuthorizations = await _ipFilterService.GetValidAsync(user);
            return validAuthorizations.Select(a => IPAddress.Parse(a.IpAddress)).ToHashSet();
        }
    }
}
