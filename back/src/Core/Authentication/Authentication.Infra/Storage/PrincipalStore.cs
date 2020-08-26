using Authentication.Domain;
using Authentication.Infra.Services;
using Lucca.Core.Authentication.Abstractions.Stores;
using Lucca.Core.Shared.Domain.Contracts.Principals;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Authentication.Infra.Storage
{
    public class PrincipalStore : IPrincipalStore<ClaimsPrincipal>
    {
        private readonly AuthenticationRemoteService _authService;

        public PrincipalStore(AuthenticationRemoteService authService)
        {
            _authService = authService;
        }

        public ClaimsPrincipal GetPrincipal(PrincipalType type, Guid token)
        {
            return GetPrincipalAsync(type, token).Result;
        }

        public async Task<ClaimsPrincipal> GetPrincipalAsync(PrincipalType type, Guid token)
        {
            switch (type)
            {
                case PrincipalType.User:
                    return SetupUserPrincipal(await _authService.GetUserPrincipalAsync(token));
                case PrincipalType.ApiKey:
                    return SetupApiKeyPrincipal(_authService.GetApiKeyPrincipal(token));
                default:
                    throw new NotImplementedException(type.ToString());
            }
        }

        private CloudControlUserClaimsPrincipal SetupUserPrincipal(Principal principal)
            => principal == null ? null : new CloudControlUserClaimsPrincipal(principal);

        private CloudControlApiKeyClaimsPrincipal SetupApiKeyPrincipal(ApiKey principal)
            => principal == null ? null : new CloudControlApiKeyClaimsPrincipal(principal);
    }
}
