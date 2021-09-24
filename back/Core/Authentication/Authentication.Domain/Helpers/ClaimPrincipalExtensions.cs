using System;
using System.Security.Claims;

namespace Authentication.Domain.Helpers
{
    public static class ClaimPrincipalExtensions
    {

        public static int? GetAuthorIdOnlyWhenUser(this ClaimsPrincipal principal)
        {
            if (!(principal is CloudControlUserClaimsPrincipal user))
            {
                return null;
            }

            return user.UserId.Value;
        }

        public static int GetAuthorId(this ClaimsPrincipal principal)
        {
            return principal switch
            {
                CloudControlUserClaimsPrincipal user => user.UserId.Value,
                CloudControlApiKeyClaimsPrincipal apiKey => apiKey.ApiKey.Id,
                _ => throw new NotImplementedException()
            };
        }

        public static int GetDistributorId(this ClaimsPrincipal principal)
        {
            return principal switch
            {
                CloudControlUserClaimsPrincipal user => user.User.DistributorId,
                CloudControlApiKeyClaimsPrincipal apiKey => apiKey.ApiKey.DistributorId,
                _ => throw new NotImplementedException()
            };
        }

        public static string GetApiKeyStorableId(this ClaimsPrincipal principal)
        {
            return principal switch
            {
                CloudControlApiKeyClaimsPrincipal apiKey => apiKey.ApiKey.StorableId,
                CloudControlUserClaimsPrincipal _ => null,
                _ => throw new NotImplementedException()
            };
        }

    }
}
