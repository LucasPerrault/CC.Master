using Authentication.Domain;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;

namespace Instances.Domain.Demos
{
    public interface IDemoRightsFilter
    {
        Expression<Func<Demo, bool>> GetDefaultReadFilter(ClaimsPrincipal principal);
    }

    public class DemoRightsFilter : IDemoRightsFilter
    {
        public Expression<Func<Demo, bool>> GetDefaultReadFilter(ClaimsPrincipal principal)
        {

            return principal switch
            {
                // TODO : Faux pour le moment
                CloudControlUserClaimsPrincipal userPrincipal => (Demo d) => d.Distributor.Code == userPrincipal.User.DepartmentCode,
                CloudControlApiKeyClaimsPrincipal apiKey => (Demo d) => true,
                _ => throw new ApplicationException("Unhandled ClaimsPrincipal type")
            };
        }
    }
}
