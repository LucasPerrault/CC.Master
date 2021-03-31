#nullable enable

using Authentication.Domain;
using Lucca.Core.Rights.Abstractions.Principals;
using Lucca.Core.Rights.RightsHelper;
using System;
using System.Security.Claims;

namespace Rights.Infra.Services
{
    public class RightsHelper : ClaimsPrincipalRightsHelper
    {
        internal const int CloudControlAppInstanceId = 24;

        public RightsHelper(IServiceProvider provider)
            : base(provider)
        { }

        protected override bool IsUser(ClaimsPrincipal principal, out IUser? user)
        {
            if (principal is CloudControlUserClaimsPrincipal userClaimsPrincipal)
            {
                user = userClaimsPrincipal.User;
                return true;
            }

            user = null;
            return false;
        }

        protected override bool IsApiKey(ClaimsPrincipal principal, out IApiKey? apiKey)
        {
            if (principal is CloudControlApiKeyClaimsPrincipal userClaimsPrincipal)
            {
                apiKey = userClaimsPrincipal.ApiKey;
                return true;
            }

            apiKey = null;
            return false;
        }
    }
}
