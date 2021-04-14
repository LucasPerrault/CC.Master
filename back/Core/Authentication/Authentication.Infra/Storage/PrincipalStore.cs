using Authentication.Domain;
using Authentication.Infra.Services;
using Lucca.Core.Authentication.Abstractions.Stores;
using Lucca.Core.Shared.Domain.Contracts.Principals;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Authentication.Infra.Storage
{
    public class PrincipalStore : IPrincipalStore<ClaimsPrincipal>
    {
        private readonly UserAuthenticationRemoteService _userAuthService;
        private readonly ApiKeyAuthenticationRemoteService _apiKeyAuthService;
        private readonly ILogger<PrincipalStore> _logger;

        public PrincipalStore
        (
            UserAuthenticationRemoteService userAuthService,
            ApiKeyAuthenticationRemoteService apiKeyAuthService,
            ILogger<PrincipalStore> logger
        )
        {
            _userAuthService = userAuthService;
            _apiKeyAuthService = apiKeyAuthService;
            _logger = logger;
        }

        public ClaimsPrincipal GetPrincipal(PrincipalType type, Guid token)
        {
            return GetPrincipalAsync(type, token).Result;
        }

        public async Task<ClaimsPrincipal> GetPrincipalAsync(PrincipalType type, Guid token)
        {
            try
            {
                return type switch
                {
                    PrincipalType.User => SetupUserPrincipal(await _userAuthService.GetUserPrincipalAsync(token)),
                    PrincipalType.ApiKey => SetupApiKeyPrincipal(await _apiKeyAuthService.GetApiKeyPrincipalAsync(token)),
                    _ => throw new ApplicationException($"Principal type not supported : {type}")
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not resolve principal for incoming request");
                throw;
            }
        }

        private CloudControlUserClaimsPrincipal SetupUserPrincipal(Principal principal)
            => principal == null ? null : new CloudControlUserClaimsPrincipal(principal);

        private CloudControlApiKeyClaimsPrincipal SetupApiKeyPrincipal(ApiKey principal)
            => principal == null ? null : new CloudControlApiKeyClaimsPrincipal(principal);
    }
}
