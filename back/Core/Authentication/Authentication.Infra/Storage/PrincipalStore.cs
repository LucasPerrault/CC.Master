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
        private readonly UserAuthenticationRemoteService _userAuthService;
        private readonly ApiKeyAuthenticationRemoteService _apiKeyAuthService;

        public PrincipalStore(UserAuthenticationRemoteService userAuthService, ApiKeyAuthenticationRemoteService apiKeyAuthService)
        {
            _userAuthService = userAuthService;
            _apiKeyAuthService = apiKeyAuthService;
        }

        public ClaimsPrincipal GetPrincipal(PrincipalType type, Guid token)
        {
            return GetPrincipalAsync(type, token).Result;
        }

        public async Task<ClaimsPrincipal> GetPrincipalAsync(PrincipalType type, Guid token)
        {
            return type switch
            {
                PrincipalType.User => SetupUserPrincipal(await _userAuthService.GetUserPrincipalAsync(token)),
                PrincipalType.ApiKey => SetupApiKeyPrincipal(await _apiKeyAuthService.GetApiKeyPrincipalAsync(token)),
                _ => throw new NotImplementedException(type.ToString())
            };
        }

        private CloudControlUserClaimsPrincipal SetupUserPrincipal(Principal principal)
            => principal == null ? null : new CloudControlUserClaimsPrincipal(principal);

        private CloudControlApiKeyClaimsPrincipal SetupApiKeyPrincipal(ApiKey principal)
            => principal == null ? null : new CloudControlApiKeyClaimsPrincipal(principal);
    }
}
