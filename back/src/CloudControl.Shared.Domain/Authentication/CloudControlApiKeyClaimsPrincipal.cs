using Lucca.Core.Authentication;
using Lucca.Core.Shared.Domain.Contracts.Principals;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace CloudControl.Shared.Domain.Authentication
{
    public class CloudControlApiKeyClaimsPrincipal : ClaimsPrincipal
    {
        public CloudControlApiKeyClaimsPrincipal(ApiKey principal)
            : base(new ClaimsIdentity(
                new List<Claim>
                {
                    new Claim(nameof(Authentication.ApiKey.Type), principal.Type.ToString()),
                    new Claim(nameof(Authentication.ApiKey.Token), principal.Token.ToString()),
                    new Claim(nameof(Authentication.ApiKey.Name), principal.Name)
                }, AuthenticationExtensions.LuccaScheme))
        {
            ApiKey = principal;
        }

        public PrincipalType Type => ApiKey.Type;
        public Guid Token => ApiKey.Token;
        public ApiKey ApiKey { get; }
    }
}
