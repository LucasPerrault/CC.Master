using Authentication.Domain;
using Distributors.Domain.Models;
using System.Security.Claims;
using Users.Domain;

namespace Testing.Infra
{
    public class TestPrincipal
    {
        private const int DefaultTestDepartmentId = 1;

        public static ClaimsPrincipal NewUser() => NewUser(new Distributor { Id = DefaultTestDepartmentId });

        public static ClaimsPrincipal NewUser(Distributor distributor) => new CloudControlUserClaimsPrincipal(new Principal
        {
            UserId = 1,
            User = new User
            {
                Distributor = distributor,
                FirstName = "Jean",
                LastName = "Bombeur",
            }
        });
    }
}
